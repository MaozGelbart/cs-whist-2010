using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.API
{
    /// <summary>
    /// Shape of a card ( or a bid)
    /// </summary>
    public enum Suit
    {
        Clubs = 1,          // tiltan
        Diamonds = 2,       
        Hearts = 3,
        Spades = 4          // ale
    }

    [Serializable]
    public struct Card
    {
        public Card(Suit suit, int value)
            :this()
        {
            Suit = suit;
            if (value <= 1 || value > 14)
                throw new Exception("Card created with wrong arguments");
            Value = value;
        }

        public Suit Suit { get; set; }

        /// <summary>
        /// From 2 to 14(Ace)
        /// </summary>
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Card)
            {
                Card c = (Card)obj;
                return c.Suit == this.Suit && c.Value == this.Value;
            }
            else
                return false;
        }

        public bool IsEmpty()
        {
            return this.Value == 0 || this.Suit == 0;
        }

        /// <summary>
        /// Hash code for a card is 2 bits for suit (1,2,3,4) the rest are for value
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)this.Suit + this.Value * 4;
        }
    }
}