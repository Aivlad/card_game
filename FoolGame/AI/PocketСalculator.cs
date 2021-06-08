using FoolGame.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoolGame.AI
{
    class PocketСalculator
    {
        PocketCell Head;
        readonly int Depth;

        public PocketСalculator
            (
                PlayingCard trump,
                List<PlayingCard> deckBox,
                List<PlayingCard> handPlayer1Box,
                List<PlayingCard> tableBox,
                List<PlayingCard> discardBox,
                int depth
            )
        {
            Head = new PocketCell(trump, deckBox, handPlayer1Box, tableBox, discardBox);
            Depth = depth;
            CalculationProcess();
        }

        private void CalculationProcess()
        {

        }

        /// <summary>
        /// Перенести карту из одного списка в другой
        /// </summary>
        /// <param name="fromList">Список источник</param>
        /// <param name="toList">Список получатель</param>
        /// <param name="index">Индекс переходной карты</param>
        void Moving(List<PlayingCard> fromList, List<PlayingCard> toList, int index)
        {
            toList.Add(fromList[index]);
            fromList.RemoveAt(index);
        }

        void AttackFirsMove(PocketCell currentCell)
        {
            /*
                В методе должны:
                взять поочередно каждый элемент в руке и положить на стол             
             */

            var countCardsInHand = currentCell.HandPlayer.Count;
            for (var i = 0; i < countCardsInHand; i++)
            {
                var hand = new List<PlayingCard>(currentCell.HandPlayer);
                var table = new List<PlayingCard>(currentCell.TableBox);

                Moving(hand, table, i);

                new PocketCell(currentCell, currentCell.Trump, currentCell.DeckBox, hand, table, currentCell.DiscardBox);
            }
        }


        void AttackToss(PocketCell currentCell)
        {
            /*
                В методе должны:
                1. определить карты, которые можно подкинуть        
                2. определенные карты поочередно взять из руки и положить на стол
             */
            var hand = new List<PlayingCard>(currentCell.HandPlayer);
            var table = new List<PlayingCard>(currentCell.TableBox);

            // поиск значений, которыми можем атаковать
            var valuesPotencialCard = hand
                .Select(item => item.value)
                .Intersect(table.Select(item => item.value));
            if (valuesPotencialCard.Any())
            {
                // определяем какие карты подходят для подкидывания
                var potencialCard = new List<PlayingCard>();
                foreach (var card in hand)
                {
                    if (valuesPotencialCard.Contains(card.value))
                        potencialCard.Add(card);
                }

                // поочередно кладем на стол
                var count = potencialCard.Count;
                for (var i = 0; i < count; i++)
                {
                    hand = new List<PlayingCard>(currentCell.HandPlayer);
                    table = new List<PlayingCard>(currentCell.TableBox);

                    var index = hand.IndexOf(potencialCard[i]);

                    Moving(hand, table, i);

                    new PocketCell(currentCell, currentCell.Trump, currentCell.DeckBox, hand, table, currentCell.DiscardBox);
                }
            }
        }


        void AttackPass()
        {
            /*
                В методе должны:
                1. карты со стола переложить в отбой
                2. все неизвестные карты перебором додать игроку в руку
             */

            // ------------- задача перебора
        }

        void ProtectionPickUp(PocketCell currentCell)
        {
            /*
                В методе должны:
                переложить карты со стола в руку игроку
             */
            var hand = new List<PlayingCard>(currentCell.HandPlayer);
            var table = new List<PlayingCard>(currentCell.TableBox);

            hand.AddRange(table);
            table.Clear();

            new PocketCell(currentCell, currentCell.Trump, currentCell.DeckBox, hand, table, currentCell.DiscardBox);
        }

        void ProtectionFightOff(PocketCell currentCell)
        {
            /*
                В методе должны:
                1. определить карты, которыми можно отбиться
                2. определенные карты поочередно взять из руки и положить на стол
             */
            var hand = new List<PlayingCard>(currentCell.HandPlayer);
            var table = new List<PlayingCard>(currentCell.TableBox);

            var currentTrump = currentCell.Trump;

            // карта, которую нужно побить
            var card = table[table.Count - 1];

            // поиск старших карт по масти
            var highCards = hand.Where(item => item.suit == card.suit && item.weight > card.weight);
            // поиск карт козырной карты
            var junTrumpCard = hand.Where(item => item.suit == currentTrump.suit);

            // отбор нужных карт
            var potencialCard = new List<PlayingCard>();
            if (highCards.Any())
            {
                potencialCard.AddRange(highCards);
            }
            if (card.suit != currentTrump.suit && junTrumpCard.Any())
            {
                potencialCard.AddRange(junTrumpCard);
            }

            // поочередно кладем на стол
            var count = potencialCard.Count;
            for (var i = 0; i < count; i++)
            {
                hand = new List<PlayingCard>(currentCell.HandPlayer);
                table = new List<PlayingCard>(currentCell.TableBox);

                var index = hand.IndexOf(potencialCard[i]);

                Moving(hand, table, i);

                new PocketCell(currentCell, currentCell.Trump, currentCell.DeckBox, hand, table, currentCell.DiscardBox);
            }
        }


        class PocketCell
        {
            public int LevelCell;

            int WeightCell;

            public int CountChildren;

            public PlayingCard Trump;
            public List<PlayingCard> DeckBox;
            public List<PlayingCard> HandPlayer;
            public List<PlayingCard> TableBox;
            public List<PlayingCard> DiscardBox;

            public PocketCell Parent;
            public List<PocketCell> Children;

            /// <summary>
            /// Создание родительского элемента
            /// </summary>
            /// <param name="trump">Козырь в игре</param>
            /// <param name="deckBox">Список карт из колоды (источник неизвестных карт)</param>
            /// <param name="handPlayer">Список карт игрока</param>
            /// <param name="tableBox">Список карт на столе</param>
            /// <param name="discardBox">Список карт отбоя</param>
            public PocketCell
                (
                    PlayingCard trump,
                    List<PlayingCard> deckBox,
                    List<PlayingCard> handPlayer,
                    List<PlayingCard> tableBox,
                    List<PlayingCard> discardBox
                )
            {
                LevelCell = 1;

                WeightCell = int.MinValue;

                CountChildren = 0;

                Trump = trump;
                DeckBox = new List<PlayingCard>(deckBox);
                HandPlayer = new List<PlayingCard>(handPlayer);
                TableBox = new List<PlayingCard>(tableBox);
                DiscardBox = new List<PlayingCard>(discardBox);

                Parent = null;
                Children = new List<PocketCell>();
            }

            /// <summary>
            /// Создание дочернего элемента
            /// </summary>
            /// <param name="trump">Козырь в игре</param>
            /// <param name="parent">Родительский элемент ячейки (предыдущее состояние)</param>
            /// <param name="deckBox">Список карт из колоды (источник неизвестных карт)</param>
            /// <param name="handPlayer">Список карт игрока</param>
            /// <param name="tableBox">Список карт на столе</param>
            /// <param name="discardBox">Список карт отбоя</param>
            public PocketCell
            (
                PocketCell parent,
                PlayingCard trump,
                List<PlayingCard> deckBox,
                List<PlayingCard> handPlayer,
                List<PlayingCard> tableBox,
                List<PlayingCard> discardBox
            )
            {
                LevelCell = parent.LevelCell + 1;

                Parent = parent;
                parent.Children.Add(this);
                parent.CountChildren = parent.Children.Count;

                WeightCell = int.MinValue;

                CountChildren = 0;

                Trump = trump;
                DeckBox = new List<PlayingCard>(deckBox);
                HandPlayer = new List<PlayingCard>(handPlayer);
                TableBox = new List<PlayingCard>(tableBox);
                DiscardBox = new List<PlayingCard>(discardBox);

                Children = new List<PocketCell>();
            }
        }
    }
}

