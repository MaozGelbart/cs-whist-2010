using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;

namespace Server.Clients
{
    internal class HighBidder : DumbPlayer
    {
        public override void RequestBid()
        {
            Bid? bid = null;
            if (this.status.Biddings.Count(b => b != null) == 0)
                bid = new API.Bid { Suit = Suit.Hearts, Amount = 6 };
            InvokeGetBidCompleted(bid);
        }

        public override void RequestDeclare()
        {
            int matchHighestBid = (from b in this.status.Biddings
                                   where b != null
                                   select b.Value.Amount).FirstOrDefault();
            if (matchHighestBid == 0)
                matchHighestBid = 6;
            InvokeOnGetContractCompleted(matchHighestBid);
        }

        public override string Name
        {
            get
            {
                return base.Name + "er";
            }
        }
    }
}