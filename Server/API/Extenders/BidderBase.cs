using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Inherit this class to implement bidder part
    /// </summary>
    public abstract class BidderBase : PlayerPartExtender, IBidder
    {
        public abstract Bid? RequestBid();

        public abstract int RequestDeclare();
    }
}
