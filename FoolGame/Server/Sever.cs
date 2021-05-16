using FoolGame.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FoolGame.Server
{
    class Sever
    {
        // данные (карты), хранимые сервером
        readonly DeckCards Deck;
        readonly List<PlayingCard> HandPlayer1 = new List<PlayingCard>();
        readonly List<PlayingCard> HandPlayer2 = new List<PlayingCard>();
        readonly List<PlayingCard> Table = new List<PlayingCard>();
        readonly List<PlayingCard> PickDown = new List<PlayingCard>();

        // методы с оболочки
        public Action<string> PrintLogs;
        public Action<RichTextBox, List<PlayingCard>> PrintCardsInTextBox;
        readonly List<RichTextBox> AllBoxes;

        // активынй игрок
        int ActivePlayer;

        public Sever(Action<string> printLogs, Action<RichTextBox, List<PlayingCard>> printCardsInTextBox, List<RichTextBox> obj)
        {
            // методы взаимодействия с формой
            PrintLogs = printLogs;
            PrintCardsInTextBox = printCardsInTextBox;
            AllBoxes = obj;
            PrintLogs("Сервер инициализирован");

            // работа с колодой
            Deck = new DeckCards(); // сама колода
            PrintCardsInTextBox(AllBoxes[0], new List<PlayingCard> { Deck.ViewTrump() });   // показать козырь

            // выдача карт на руки
            for (var i = 0; i < 6; i++) 
            {
                HandPlayer1.Add(Deck.TakeCard());
                HandPlayer2.Add(Deck.TakeCard());
            }
            ViewState();
            PrintLogs("Карты розданы");

            // определение активного игрока
            InitActivePlayer();
            PrintLogs("Сейчас ходит игрок " + ActivePlayer);
        }

        /// <summary>
        /// Вывести все списки на форму
        /// </summary>
        void ViewState()
        {
            PrintCardsInTextBox(AllBoxes[1], HandPlayer1);
            PrintCardsInTextBox(AllBoxes[2], HandPlayer2);
            PrintCardsInTextBox(AllBoxes[5], Table);
            PrintCardsInTextBox(AllBoxes[3], Deck.GetDeck());
            PrintCardsInTextBox(AllBoxes[4], PickDown);
        }

        /// <summary>
        /// Определить кто первый ходит, условия: 1) меньший козырь, иначе 2) наибольшая карта, иначе 3) случайны игрок
        /// </summary>
        void InitActivePlayer()
        {
            // козырь
            var trump = Deck.ViewTrump();
            // выборка козырей
            var hand1 = HandPlayer1.Where(item => item.suit == trump.suit);
            var hand2 = HandPlayer2.Where(item => item.suit == trump.suit);

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
                if (HandPlayer1.Max(item => item.weight) > HandPlayer2.Max(item => item.weight))
                    ActivePlayer = 1;
                else if (HandPlayer1.Max(item => item.weight) > HandPlayer2.Max(item => item.weight))
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
