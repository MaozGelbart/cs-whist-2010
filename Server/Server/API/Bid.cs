using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.API
{
    [Serializable]
    public struct Bid
    {
        /// <summary>
        /// Strong shape for bidding, null if suit-less
        /// </summary>
        public Suit? Suit { get; set; }

        /// <summary>
        /// number of tricks, from 0 to 13
        /// </summary>
        public int Amount { get; set; }
    }
}