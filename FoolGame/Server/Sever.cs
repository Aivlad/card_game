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
        enum UserAction
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
        Action<string> PrintLogs;
        Action<RichTextBox, List<PlayingCard>> PrintCardsInTextBox;
        readonly List<RichTextBox> AllBoxes;

        // активынй игрок
        int ActivePlayer;

        // AI 
        Lori Player1;

        /// <param name="printLogs">Делегат вывода данных в окно логов</param>
        /// <param name="printCardsInTextBox">Делегат вывода в указанный RichTextBox список карт</param>
        /// <param name="obj">Доступные RichTextBox окна</param>
        public Sever(Action<string> printLogs, Action<RichTextBox, List<PlayingCard>> printCardsInTextBox, List<RichTextBox> obj)
        {
            // методы взаимодействия с формой
            PrintLogs = printLogs;
            PrintCardsInTextBox = printCardsInTextBox;
            AllBoxes = obj;
            PrintLogs("Сервер инициализирован");

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
            Player1 = new Lori(DeckBox.ViewTrump());
        }

        /// <summary>
        /// Вызов действия (AI (Player 1) vs человек (Player 2))
        /// </summary>
        /// <param name="indexString">Входной индекс (определяет в активной руке номер карты, которой нужно походить; если -1, то вызов альтернативных действий)</param>
        public void MakeMove(string indexString, bool mock)
        {
            if ((HandPlayer1Box.Count == 0 || HandPlayer2Box.Count == 0) && DeckBox.Count() == 0)
            {
                PrintLogs("Игра окончена");
                return;
            }

            bool answer = false;
            if (ActivePlayer == 1)
            {
                // определение того, что нужно ожидать от ИИ
                var currentUserAction = UserAction.None;
                if (TableBox.Count == 0)
                    currentUserAction = UserAction.FirstMove;
                else if (TableBox.Count % 2 == 0)
                    currentUserAction = UserAction.Attack;
                else
                    currentUserAction = UserAction.Protection;

                // через количества определим потом какие изменения были
                var countHandPlayerBoxOld = HandPlayer1Box.Count;
                var countTableBoxOld = TableBox.Count;


                // запрос хода у ИИ
                PrintLogs($"{Player1} зпрос хода");
                var solution = Player1.MakeMove(HandPlayer1Box, TableBox, DeckBox.Count());

                // проверка что изменилось:
                // UserAction.FirstMove не требует доп. проверки
                // UserAction.Attack требует доп. проверки: подкинули или пасанули
                // UserAction.Protection требует доп. проверки: забрали или отбились
                var countHandPlayerBoxNew = HandPlayer1Box.Count;
                var countTableBoxNew = TableBox.Count;
                if (currentUserAction == UserAction.FirstMove)
                {
                    PrintLogs($"{Player1} провел UserAction.FirstMove");
                }
                else if (currentUserAction == UserAction.Attack)
                {
                    if (countHandPlayerBoxNew == countHandPlayerBoxOld && countTableBoxNew == countTableBoxOld)
                    {
                        // нужно "сделать" пас:
                        // - скинуть карты со стола
                        // - закинуть карты в руки игрокам (если есть)
                        Discard();
                        PrintLogs($"{Player1} провел UserAction.Attack: Пас");
                    }
                    else
                    {
                        PrintLogs($"{Player1} провел UserAction.Attack: Подкинуть");
                    }
                }
                else if (currentUserAction == UserAction.Protection)
                {
                    if (countTableBoxNew == 0 && countTableBoxOld + countHandPlayerBoxOld == countHandPlayerBoxNew)
                    {
                        PrintLogs($"{Player1} провел UserAction.Protection: Забрать карты со стола");
                    }
                    else
                    {
                        PrintLogs($"{Player1} провел UserAction.Attack: Отбиться");
                    }
                }
                else
                {
                    throw new Exception("Такое состояние не должно определяться, что-то пошло не так");
                }
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

            if (answer)
                ChangeActivePlayer();
        }


        /// <summary>
        /// Вызов действия (человек (Player 1) vs человек (Player 2))
        /// </summary>
        /// <param name="indexString">Входной индекс (определяет в активной руке номер карты, которой нужно походить; если -1, то вызов альтернативных действий)</param>
        public void MakeMove(string indexString)
        {
            int index = ConvertIndex(indexString);

            if ((HandPlayer1Box.Count == 0 || HandPlayer2Box.Count == 0) && DeckBox.Count() == 0)
            {
                PrintLogs("Игра окончена");
                return;
            }

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

            if (answer)
                ChangeActivePlayer();
        }

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

            // дураная проверка т.к. здесь нужно знать просто кому карты докинуть из колоды
            if (ActivePlayer == 1)
                DealCards(HandPlayer1Box, HandPlayer2Box);
            else
                DealCards(HandPlayer2Box, HandPlayer1Box);
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

            if (ActivePlayer == 1)
                DealCards(HandPlayer1Box, HandPlayer2Box);
            else
                DealCards(HandPlayer2Box, HandPlayer1Box);
            return true;
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
