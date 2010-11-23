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

        #region IComparable<Bid> Members

        public int CompareTo(Bid other)
        {
            if( other.Amount > this.Amount)
                return -1;
            if (other.Amount < this.Amount)
                return 1;
            if (other.Suit > this.Suit)
                return -1;
            if (other.Suit < this.Suit)
                return 1;
            return 0;
        }

        #endregion
    }
}