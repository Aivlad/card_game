using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoolGame.Deck
{
    class DeckCards
    {
        /// <summary>
        /// вся колода
        /// </summary>
        readonly Dictionary<Suit, List<PlayingCard>> Pack = new Dictionary<Suit, List<PlayingCard>>(4);
        /// <summary>
        /// колода карт добора
        /// </summary>
        readonly List<PlayingCard> Deck = new List<PlayingCard>();
        /// <summary>
        /// карта, обозначающая текущий козырь
        /// </summary>
        PlayingCard CurrentTrump;

        /// <summary>
        /// Сформировать обычную колоду
        /// </summary>
        public DeckCards()
        {
            InitPack();
            InitDeck();
            InitTrump();
        }

        /// <summary>
        /// Сформировать подстроенную колоду
        /// </summary>
        public DeckCards(PlayingCard trump)
        {
            InitPack();
            InitDeck();
            InitTrump(trump);
        }

        public List<PlayingCard> GetDeck()
        {
            return Deck;
        }

        public int Count()
        {
            return Deck.Count;
        }

        /// <summary>
        /// Показать козырную карту
        /// </summary>
        public PlayingCard ViewTrump()
        {
            return CurrentTrump;
        }

        /// <summary>
        /// Взять карту из колоды
        /// </summary>
        /// <returns>карта или null, если колода пустая</returns>
        public PlayingCard TakeCard()
        {
            if (Deck.Count == 0)
                return null;

            var card = Deck[0];
            Deck.RemoveAt(0);

            return card;
        }

        /// <summary>
        /// Инициализация всех карт
        /// </summary>
        void InitPack()
        {
            for (var i = 0; i < 4; i++)
            {
                List<PlayingCard> value = new List<PlayingCard>();
                var weight = -400;
                for (var j = 0; j < 9; j++)
                {
                    value.Add(new PlayingCard((Suit)i, (Value)j, weight));
                    weight += 100;
                }
                Pack.Add((Suit)i, value);
            }
        }

        /// <summary>
        /// Инициализация перемешанной колоды
        /// </summary>
        void InitDeck()
        {
            // просто добавление карт
            var tmpHand = new List<PlayingCard>();
            foreach (var suit in Pack)
            {
                foreach (var value in suit.Value)
                {
                    tmpHand.Add(value);
                }
            }

            // перемешиваем
            var rnd = new Random();
            while (tmpHand.Count > 0)
            {
                var num = rnd.Next(0, tmpHand.Count);
                Deck.Add(tmpHand[num]);
                tmpHand.RemoveAt(num);
            }
        }

        /// <summary>
        /// Определить козырную масть
        /// </summary>
        void InitTrump()
        {
            var indexLastCard = Deck.Count - 1;
            CurrentTrump = Deck[indexLastCard];
            AddWeightCards();
        }

        /// <summary>
        /// Указать текущий козырь
        /// </summary>
        void InitTrump(PlayingCard trump)
        {
            CurrentTrump = trump;
            AddWeightCards();
        }

        /// <summary>
        /// Добавить вес козырным картам
        /// </summary>
        void AddWeightCards()
        {
            // добавляем вес козырям
            foreach (var card in Pack[CurrentTrump.suit])
            {
                card.weight += 900;
            }
        }

        public override string ToString()
        {
            var output = "";
            foreach (var card in Deck)
                output += card + "\n";
            output += $"{Deck.Count}\n";
            return output;
        }
    }
}
