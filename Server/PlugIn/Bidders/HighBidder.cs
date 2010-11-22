using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn
{
    public class HighBidder : PlayerPartExtender<IBidder>, IBidder
    {
        #region IBidder Members

        public void RequestBid()
        {
            int matchHighestBid = (from b in this.CurrentRoundStatus.Biddings
                                   where b != null
                                   orderby b.Value descending
                                   select b.Value.Amount).FirstOrDefault() + 1;
            if (matchHighestBid == 1)
                matchHighestBid = 6;
            Bid myBid = new Bid { Suit = Suit.Spades, Amount = matchHighestBid };
            if( OnGetBidCompleted != null)
                OnGetBidCompleted(this, new RecieveBidEventArgs(myBid));
        }

        public void RequestDeclare()
        {
            int matchHighestBid = (from b in this.CurrentRoundStatus.Biddings
                                   where b != null
                                   orderby b.Value descending
                                   select b.Value.Amount).FirstOrDefault();
            if (matchHighestBid == 0)
                matchHighestBid = 6;
            if(OnGetContractCompleted != null)
                OnGetContractCompleted(this, new RecieveContractEventArgs(matchHighestBid));
        }

        public event EventHandler<RecieveBidEventArgs> OnGetBidCompleted;

        public event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;

        #endregion
    }
}
