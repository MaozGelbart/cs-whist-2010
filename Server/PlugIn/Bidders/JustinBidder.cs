using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn.Bidders
{
    /// <summary>
    /// My name is Bidder, Justin Bidder (and I love maoz, we are gayz!)
    /// </summary>
    class JustinBidder : BidderBase
    {
        public override Bid? RequestBid()
        {
            return null;
        }

        public override int RequestDeclare()
        {
            int bidders = 0, total = 0;

            for (PlayerSeat p = PlayerSeat.West; p <= PlayerSeat.East; p++)
            {
                if (CurrentRoundStatus.Biddings[(int)p].HasValue)
                {
                    total += CurrentRoundStatus.Biddings[(int)p].Value.Amount;
                    bidders++;
                }
            }

            if (bidders == 3 && total == 13)
            {
                return 1;
            }

            
            return 0;
        }
    }
}
