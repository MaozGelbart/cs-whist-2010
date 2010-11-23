using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn
{
    public class HighBidder : BidderBase
    {
        #region IBidder Members

        public override Bid? RequestBid()
        {
            int matchHighestBid = (from b in this.CurrentRoundStatus.Biddings
                                   where b != null
                                   orderby b.Value descending
                                   select b.Value.Amount).FirstOrDefault() + 1;
            if (matchHighestBid == 1)
                matchHighestBid = 6;
            if (matchHighestBid > 13)
                return null;
            return new Bid { Suit = Suit.Spades, Amount = matchHighestBid };
        }

        public override int RequestDeclare()
        {
            int matchHighestBid = (from b in this.CurrentRoundStatus.Biddings
                                   where b != null
                                   orderby b.Value descending
                                   select b.Value.Amount).FirstOrDefault();
            if (matchHighestBid == 0)
                matchHighestBid = 6;
            return matchHighestBid;
        }

        #endregion
    }
}
