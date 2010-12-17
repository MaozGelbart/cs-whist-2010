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
            return 0;
        }
    }
}
