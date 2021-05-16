using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoolGame.Deck
{
    enum Suit   // масть
    {
        Hearts,     // черви
        Clubs,      // трефы
        Diamonds,   // бубны
        Spades      // пики
    }

    enum Value  // значение карты
    {
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    class PlayingCard   // игральная карта
    {
        public Suit suit;   // масть
        public Value value;  // значение
        public int weight;  // вес

        public PlayingCard(Suit suit, Value value, int weight = 0)
        {
            this.suit = suit;
            this.value = value;
            this.weight = weight;
        }

        public override string ToString()
        {
            return $"{suit}\t{value}\t({weight})";
        }
    }

    class PlayingCardComparer : IEqualityComparer<PlayingCard>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(PlayingCard x, PlayingCard y)
        {

            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            //Check whether the products' properties are equal.
            return x.suit == y.suit && x.value == y.value && x.weight == y.weight;
        }

        public int GetHashCode(PlayingCard card)
        {
            if (card is null) return 0;

            int hashSuit = card.suit.GetHashCode();
            int hashValue = card.value.GetHashCode();
            int hashWeight = card.weight.GetHashCode();

            return hashSuit ^ hashValue ^ hashWeight;
        }
    }
}
