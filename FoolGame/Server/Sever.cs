using FoolGame.AI;
using FoolGame.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FoolGame.Server
{
    class Sever
    {
        /// <summary>
        /// Режим игры
        /// </summary>
        enum GameMode
        {
            HumanVsHuman,
            HumanVsAi,
            AiVsAi
        }

        /// <summary>
        /// Действия, совершаемые ИИ
        /// </summary>
        enum ActionAI
        {
            None,
            FirstMove,
            Protection,
            Attack
        }

        // данные (карты), хранимые сервером
        readonly DeckCards DeckBox;
        readonly List<PlayingCard> HandPlayer1Box = new List<PlayingCard>();
        readonly List<PlayingCard> HandPlayer2Box = new List<PlayingCard>();
        readonly List<PlayingCard> TableBox = new List<PlayingCard>();
        readonly List<PlayingCard> DiscardBox = new List<PlayingCard>();

        // методы с оболочки
        readonly Action<string> PrintLogs;
        readonly Action<RichTextBox, List<PlayingCard>> PrintCardsInTextBox;
        readonly List<RichTextBox> AllBoxes;

        // активынй игрок
        int ActivePlayer;

        // AI 
        readonly Lori Player1;
        readonly Lori Player2;

        // game mode
        readonly GameMode Mode;

        /// <param name="printLogs">Делегат вывода данных в окно логов</param>
        /// <param name="printCardsInTextBox">Делегат вывода в указанный RichTextBox список карт</param>
        /// <param name="obj">Доступные RichTextBox окна</param>
        public Sever(
            Action<string> printLogs, 
            Action<RichTextBox, List<PlayingCard>> printCardsInTextBox, 
            List<RichTextBox> obj
            )
        {
            // методы взаимодействия с формой
            PrintLogs = printLogs;
            PrintCardsInTextBox = printCardsInTextBox;
            AllBoxes = obj;
            PrintLogs("Сервер инициализирован");

            Mode = GameMode.AiVsAi;
            PrintLogs($"Текущий режим: {Mode}");

            // работа с колодой
            DeckBox = new DeckCards(); // сама колода
            PrintCardsInTextBox(AllBoxes[0], new List<PlayingCard> { DeckBox.ViewTrump() });   // показать козырь

            // выдача карт на руки
            for (var i = 0; i < 6; i++) 
            {
                HandPlayer1Box.Add(DeckBox.TakeCard());
                HandPlayer2Box.Add(DeckBox.TakeCard());
            }
            ViewState();
            PrintLogs("Карты розданы");

            // определение активного игрока
            InitActivePlayer();
            PrintLogs("Сейчас ходит игрок " + ActivePlayer);

            // создание сервера
            Player1 = new Lori(DeckBox.ViewTrump(), "Lori One");
            Player2 = new Lori(DeckBox.ViewTrump(), "Lori Two");
        }

        /// <summary>
        /// Метод тестирования: сброс карт из колоды в отбой
        /// </summary>
        /// <param name="count">Количество карт сброса</param>
        public void DiscardingCardsFromDeck(int count)
        {
            for (var i = 0; i < count || DiscardBox.Count() > 0; i++)
            {
                DiscardBox.Add(DeckBox.TakeCard());
            }
            ViewState();
        }

        /// <summary>
        /// Вызов действия 
        /// </summary>
        /// <param name="indexString">Входной индекс (определяет в активной руке номер карты, которой нужно походить; если -1, то вызов альтернативных действий)</param>
        public void MakeMove(string indexString)
        {
            // игра окончена, если нечем играть
            if ((HandPlayer1Box.Count == 0 || HandPlayer2Box.Count == 0) && DeckBox.Count() == 0)
            {
                PrintLogs("Игра окончена");
                return;
            }

            // определение хода в зависимости от режима
            bool answer;
            if (Mode == GameMode.HumanVsHuman)
            {
                int index = ConvertIndex(indexString);
                answer = GameModeHumanVsHuman(index);
            }
            else if (Mode == GameMode.HumanVsAi)
            {
                answer = GameModeHumanVsAi(indexString);
            }
            else if (Mode == GameMode.AiVsAi)
            {
                answer = GameModeAiVsAi();
            }
            else
            {
                throw new Exception("GameMode накрылся");
            }

            if (answer)
                ChangeActivePlayer();
        }

        /// <summary>
        /// Ходы игроков при режиме GameMode.AiVsAi
        /// </summary>
        /// <returns>True при корректной обработке хода</returns>
        private bool GameModeAiVsAi()
        {
            bool answer = false;
            if (ActivePlayer == 1)
            {
                AiInteractionProcess(Player1, HandPlayer1Box);
                answer = true;
            }
            else if (ActivePlayer == 2)
            {
                AiInteractionProcess(Player2, HandPlayer2Box);
                answer = true;
            }
            else
            {
                PrintLogs($"Выскочил ActivePlayer = {ActivePlayer}");
            }
            return answer;
        }

        /// <summary>
        /// Ходы игроков при режиме GameMode.HumanVsAi
        /// </summary>
        /// <param name="indexString">Строковое представление индекса карты хода</param>
        /// <returns>True при корректной обработке хода</returns>
        private bool GameModeHumanVsAi(string indexString)
        {
            bool answer = false;
            if (ActivePlayer == 1)
            {
                AiInteractionProcess(Player1, HandPlayer1Box);
                answer = true;
            }
            else if (ActivePlayer == 2)
            {
                int index = ConvertIndex(indexString);
                answer = StrokeProcess(HandPlayer2Box, index);
            }
            else
            {
                PrintLogs($"Выскочил ActivePlayer = {ActivePlayer}");
            }

            return answer;
        }

        /// <summary>
        /// Процесс взаимодействия сервера с ИИ
        /// </summary>
        /// <param name="player">Активный ИИ</param>
        private void AiInteractionProcess(Lori player, List<PlayingCard> hand)
        {
            // определение того, что нужно ожидать от ИИ
            ActionAI currentUserAction;
            if (TableBox.Count == 0)
            {
                PrintLogs($"{player} запрос хода ActionAI.FirstMove");
                currentUserAction = ActionAI.FirstMove;
            }
            else if (TableBox.Count % 2 == 0)
            {
                PrintLogs($"{player} запрос хода ActionAI.Attack");
                currentUserAction = ActionAI.Attack;
            }
            else
            {
                PrintLogs($"{player} запрос хода ActionAI.Protection");
                currentUserAction = ActionAI.Protection;
            }

            // через количества определим потом какие изменения были
            var countHandPlayerBoxOld = hand.Count;
            var countTableBoxOld = TableBox.Count;

            // запрос хода у ИИ
            player.MakeMove(hand, TableBox, DeckBox.Count());

            // проверка что изменилось:
            // ActionAI.FirstMove не требует доп. проверки
            // ActionAI.Attack требует доп. проверки: подкинули или пасанули
            // ActionAI.Protection требует доп. проверки: забрали или отбились
            var countHandPlayerBoxNew = hand.Count;
            var countTableBoxNew = TableBox.Count;
            if (currentUserAction == ActionAI.FirstMove)
            {
                PrintLogs($"{player} провел ActionAI.FirstMove");
            }
            else if (currentUserAction == ActionAI.Attack)
            {
                if (countHandPlayerBoxNew == countHandPlayerBoxOld && countTableBoxNew == countTableBoxOld)
                {
                    PrintLogs($"{player} провел ActionAI.Attack: Пас");
                    // нужно "сделать" пас:
                    // - скинуть карты со стола
                    // - закинуть карты в руки игрокам (если есть)
                    Discard();
                }
                else if (countHandPlayerBoxNew == countHandPlayerBoxOld - 1 && countTableBoxNew == countTableBoxOld + 1)
                {
                    PrintLogs($"{player} провел ActionAI.Attack: Подкинуть");
                }
                else
                {
                    throw new Exception("Неизвестная ситуация");
                }
            }
            else if (currentUserAction == ActionAI.Protection)
            {
                if (countTableBoxNew == 0 && countTableBoxOld + countHandPlayerBoxOld == countHandPlayerBoxNew)
                {
                    PrintLogs($"{player} провел ActionAI.Protection: Забрать карты со стола");
                    DealCardsToPlayers();
                }
                else if (countHandPlayerBoxNew == countHandPlayerBoxOld - 1 && countTableBoxNew == countTableBoxOld + 1)
                {
                    PrintLogs($"{player} провел ActionAI.Attack: Отбиться");
                }
                else
                {
                    throw new Exception("Неизвестная ситуация");
                }
            }
            else
            {
                throw new Exception("Такое состояние не должно определяться, что-то пошло не так");
            }
        }

        /// <summary>
        /// Ходы игроков при режиме GameMode.HumanVsHuman
        /// </summary>
        /// <param name="index">Индекс карты хода</param>
        /// <returns>True при корректной обработке хода</returns>
        private bool GameModeHumanVsHuman(int index)
        {
            bool answer = false;
            if (ActivePlayer == 1)
            {
                answer = StrokeProcess(HandPlayer1Box, index);
            }
            else if (ActivePlayer == 2)
            {
                answer = StrokeProcess(HandPlayer2Box, index);
            }
            else
            {
                PrintLogs($"Выскочил ActivePlayer = {ActivePlayer}");
            }
            return answer;
        }


        /// <summary>
        /// Конвертирование строкового представления индекса в целый числовой
        /// </summary>
        /// <param name="indexString">Строковое представление индекса</param>
        /// <returns>Целое числовое представление индекса</returns>
        int ConvertIndex(string indexString)
        {
            try
            {
                return int.Parse(indexString);
            }
            catch
            {
                throw new Exception("Некорректное представление индекса (вы указали не число)");
            }
        }

        /// <summary>
        /// Процесс хода (вычисление необходимого действия)
        /// </summary>
        /// <param name="hand">Активная рука</param>
        /// <param name="index">Индекс активной карты в руке</param>
        /// <returns></returns>
        bool StrokeProcess(List<PlayingCard> hand, int index)
        {
            if (TableBox.Count == 0) // первая карта
            {
                return Moving(hand, TableBox, index);
            }
            else
            {
                if (TableBox.Count % 2 == 0) // подкинуть
                {
                    if (index == -1)    // пас
                    {
                        return Discard();
                    }
                    else // подкинуть
                    {
                        return Moving(hand, TableBox, index);
                    }
                }
                else // биться
                {
                    if (index == -1)    // забрать
                    {
                        return MovingToHand(hand);
                    }
                    else // отбиться
                    {
                        return Moving(hand, TableBox, index);
                    }
                }
            }
        }

        /// <summary>
        /// Перенести карты со стола в руку
        /// </summary>
        /// <param name="hand">Активня рука</param>
        /// <returns></returns>
        bool MovingToHand(List<PlayingCard> hand)
        {
            hand.AddRange(TableBox);
            TableBox.Clear();

            DealCardsToPlayers();
            return true;
        }

        /// <summary>
        /// Сброс карт в отбой
        /// </summary>
        /// <returns></returns>
        bool Discard()
        {
            DiscardBox.AddRange(TableBox);
            TableBox.Clear();

            DealCardsToPlayers();
            return true;
        }

        /// <summary>
        /// Раздать карты игрокам (авто определение последовательности раздачи)
        /// </summary>
        private void DealCardsToPlayers()
        {
            if (ActivePlayer == 1)
                DealCards(HandPlayer1Box, HandPlayer2Box);
            else
                DealCards(HandPlayer2Box, HandPlayer1Box);
        }

        /// <summary>
        /// Заполнить карты руками (первая рука заполняется до конца, потом заполняется вторая; очереднеть: кто первый ходил, тот берет первый)
        /// </summary>
        /// <param name="hand1">Рука игрока, который сейчас первый ходит</param>
        /// <param name="hand2">Рука игрока, который сейчас ходит вторым</param>
        void DealCards(List<PlayingCard> hand1, List<PlayingCard> hand2)
        {
            while (hand1.Count < 6)
            {
                var card = DeckBox.TakeCard();
                if (card == null)
                {
                    PrintLogs("Карты закончились в колоде");
                    return;
                }
                hand1.Add(card);
            }
            while (hand2.Count < 6)
            {
                var card = DeckBox.TakeCard();
                if (card == null)
                {
                    PrintLogs("Карты закончились в колоде");
                    return;
                }
                hand2.Add(card);
            }
        }

        /// <summary>
        /// Перенести карту из одного списка в другой
        /// </summary>
        /// <param name="fromList">Список источник</param>
        /// <param name="toList">Список получатель</param>
        /// <param name="index">Индекс переходной карты</param>
        /// <returns></returns>
        bool Moving(List<PlayingCard> fromList, List<PlayingCard> toList, int index)
        {
            if (index < 0 || index >= fromList.Count)
            {
                PrintLogs("Выход за границы диапазона списка");
                return false;
            }
            toList.Add(fromList[index]);
            fromList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Поменять активного игрока
        /// </summary>
        void ChangeActivePlayer()
        {
            ActivePlayer = ActivePlayer == 1 ? 2 : 1;
            ViewState();
            PrintLogs("Сейчас ActivePlayer " + ActivePlayer);
        }

        /// <summary>
        /// Вывести все списки на форму
        /// </summary>
        void ViewState()
        {
            PrintCardsInTextBox(AllBoxes[1], HandPlayer1Box);
            PrintCardsInTextBox(AllBoxes[2], HandPlayer2Box);
            PrintCardsInTextBox(AllBoxes[5], TableBox);
            PrintCardsInTextBox(AllBoxes[3], DeckBox.GetDeck());
            PrintCardsInTextBox(AllBoxes[4], DiscardBox);
        }

        /// <summary>
        /// Определить кто первый ходит, условия: 1) меньший козырь, иначе 2) наибольшая карта, иначе 3) случайны игрок
        /// </summary>
        void InitActivePlayer()
        {
            // козырь
            var trump = DeckBox.ViewTrump();
            // выборка козырей
            var hand1 = HandPlayer1Box.Where(item => item.suit == trump.suit);
            var hand2 = HandPlayer2Box.Where(item => item.suit == trump.suit);

            // проверка условий:
            if (hand1.Any() && hand2.Any()) // козыри есть у обоих
            {
                if (hand1.Min(item => item.weight) < hand2.Min(item => item.weight))
                    ActivePlayer = 1;
                else
                    ActivePlayer = 2;
            }
            else if (hand1.Any())   // козыри есть только у 1
            {
                ActivePlayer = 1;
            }
            else if (hand2.Any()) // козыри есть только у 2
            {
                ActivePlayer = 2;
            }
            else // козыри отсутствуют на руках
            {
                // проверка наибольшей карты
                if (HandPlayer1Box.Max(item => item.weight) > HandPlayer2Box.Max(item => item.weight))
                    ActivePlayer = 1;
                else if (HandPlayer1Box.Max(item => item.weight) > HandPlayer2Box.Max(item => item.weight))
                    ActivePlayer = 2;
                else // ветка случайного числа
                {
                    var rnd = new Random();
                    var id = rnd.Next(1, 3);
                    PrintLogs($"Решение по монетке: {id}");
                    ActivePlayer = id;
                }
            }
        }
    }
}
