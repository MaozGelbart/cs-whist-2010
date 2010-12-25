using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.API
{
    /// <summary>
    /// This struct uses for bidding making
    /// </summary>
    [Serializable]
    public struct Bid : IComparable<Bid>
    {
        /// <summary>
        /// Strong shape for bidding, null if suit-less
        /// </summary>
        public Suit? Suit { get; set; }

        /// <summary>
        /// number of tricks, from 0 to 13
        /// </summary>
        public int Amount { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Bid)
            {
                Bid b = (Bid)obj;
                return b.Amount == this.Amount && b.Suit == this.Suit;
            }
            return false;
        }

        public override string ToString()
        {
            return (Suit.HasValue ? Suit.Value.ToString() : "No-suit") + " " + Amount;
        }

        #region IComparable<Bid> Members

        public int CompareTo(Bid other)
        {
            if( other.Amount > this.Amount)
                return -1;
            if (other.Amount < this.Amount)
                return 1;
            int mySuit = GetBidSuitCode(this.Suit);
            int hisSuit = GetBidSuitCode(other.Suit);
            if (hisSuit > mySuit)
                return -1;
            if (hisSuit < mySuit)
                return 1;
            return 0;
        }

        public static int GetBidSuitCode(Suit? suit)
        {
            return suit.HasValue ? (int)suit.Value : 5;
        }

        public static bool operator >= (Bid self, Bid other)
        {
            return self.CompareTo(other) >= 0;
        }

        public static bool operator ==(Bid self, Bid other)
        {
            return self.CompareTo(other) == 0;
        }

        public static bool operator !=(Bid self, Bid other)
        {
            return self.CompareTo(other) != 0;
        }

        public static bool operator >(Bid self, Bid other)
        {
            return self.CompareTo(other) > 0;
        }

        public static bool operator <(Bid self, Bid other)
        {
            return self.CompareTo(other) < 0;
        }

        public static bool operator <=(Bid self, Bid other)
        {
            return self.CompareTo(other) <= 0;
        }

        /// <summary>
        /// Hash for a bid is 3 LSB is for suit (values:1-5)
        /// all the rest are for amount
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Bid.GetBidSuitCode(this.Suit) + this.Amount * 8;
        }

        #endregion
    }
}