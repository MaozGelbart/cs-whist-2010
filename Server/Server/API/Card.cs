using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.API
{
    public enum Suit
    {
        Clubs = 1,
        Diamonds = 2,
        Hearts = 3,
        Spades = 4        
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
        /// From 1 to 13
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
    }
}