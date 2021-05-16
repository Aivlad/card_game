using FoolGame.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoolGame.AI
{
    /*
        Lori - неопытный игрок
        характеристика: делает примитивные ходы, ничего особо не просчитывает
        тактика:
            - при защите: пытается отбится самой маленькой доступной картой
            - при атаке: 
                пока карт в нерозданной колоде больше определенного количества - подкидывает слабые карты
                если карт в нерозданной колоде меньше определенного количества - пытается по максимуму закидать соперника
     */
    class Lori : AI
    {
        /// <summary>
        /// Козырь в текущей игре
        /// </summary>
        PlayingCard CurrentTrump;

        /// <param name="trump">Козырь в текущей игре</param>
        public Lori(PlayingCard trump)
        {
            CurrentTrump = trump;
        }

        public override (List<PlayingCard> hand, List<PlayingCard> table) MakeMove(List<PlayingCard> hand, List<PlayingCard> table, int countDroping)
        {
            // проверка: есть ли карты в руке
            if (hand.Count == 0)
            {
                // ничего не меняем
                return (hand, table);  
            }

            // проверка: какое действие мы должны совершить
            if (table.Count == 0) 
            {
                // делаем первый ход: кидаем самую слабую карту
                var index = hand.IndexOf(hand.OrderBy(item => item.weight).First());
                Moving(hand, table, index);
                return (hand, table);
            }
            else
            {
                if (table.Count % 2 == 0) 
                {
                    // подкинуть или пасануть

                    // поиск значений, которыми можем атаковать
                    var valuesPotencialCard = hand
                        .Select(item => item.value)
                        .Intersect(table.Select(item => item.value));
                    if (valuesPotencialCard.Any())
                    {
                        // т.к. обычный linq intersect не прокатит, берем костыль через список
                        // определяем какие карты подходят для подкидывания
                        var potencialCard = new List<PlayingCard>();
                        foreach (var card in hand)
                        {
                            if (valuesPotencialCard.Contains(card.value))
                                potencialCard.Add(card);
                        }

                        // сортировка карт по весу
                        potencialCard = potencialCard.OrderBy(item => item.weight).ToList();
                        // проверка: если карт в колоде еще много, то подкидываем только легкие
                        if (countDroping > 4)
                        {                            
                            potencialCard = potencialCard.Where(item => item.weight <= 0).ToList();
                            if (potencialCard.Any())
                            {
                                var index = hand.IndexOf(potencialCard.First());
                                Moving(hand, table, index);
                                return (hand, table);
                            }    
                        }
                        else // просто закидываем соперника
                        {
                            var index = hand.IndexOf(potencialCard.First());
                            Moving(hand, table, index);
                            return (hand, table);
                        }
                    }
                    return (hand, table); // пас
                }
                else 
                {
                    // отбиться или забрать со стола

                    // карта, которую нужно побить
                    var card = table[table.Count - 1];   

                    // поиск старшей карты по масти
                    var highCards = hand
                        .Where(item => item.suit == card.suit && item.weight > card.weight)
                        .OrderBy(item => item.weight);
                    if (highCards.Any())
                    {
                        var index = hand.IndexOf(highCards.First());
                        Moving(hand, table, index);
                        return (hand, table);
                    }

                    // если карта, которую нужно отбить - не козырь, то еще можем побиться меньшим козырем в руке (если есть)
                    if (card.suit != CurrentTrump.suit)    
                    {
                        // поиск младшей козырной карты
                        var junTrumpCard = hand
                            .Where(item => item.suit == CurrentTrump.suit)
                            .OrderBy(item => item.weight);
                        if (junTrumpCard.Any())
                        {
                            var index = hand.IndexOf(junTrumpCard.First());
                            Moving(hand, table, index);
                            return (hand, table);
                        }
                    }

                    // если добрались сюда, то должны забрать карты со стола
                    Moving(hand, table);
                    return (hand, table);
                }
            }

            //throw new NotImplementedException();
        }
    }
}
