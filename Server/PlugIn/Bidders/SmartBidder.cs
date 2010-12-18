using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn.Bidders
{
    class SmartBidder : BidderBase
    {
        #region private members

        double m_highestBid = -1;
        Suit m_highestBidSuit;

        #endregion

        #region IBidder Members

        public override Bid? RequestBid()
        {
            //BUG BUG BUG..... where do you clean this (FGS) for the second round?!?!?! will always have the value of last bid!!!  BUG BUG BUG
            //if (m_highestBid == -1)
            //{
                m_highestBid = GetHighestBid(this.Cards);
                m_highestBidSuit = GetHighestBidSuit();
            //}

            int currHighestBid = (from b in this.CurrentRoundStatus.Biddings
                                   where b != null
                                   orderby b.Value descending
                                   select b.Value.Amount).FirstOrDefault();


            Suit? currSuit = (from b in this.CurrentRoundStatus.Biddings
                                  where b != null
                                  orderby b.Value descending
                                  select b.Value.Suit).FirstOrDefault();


            if (m_highestBid >= currHighestBid && m_highestBid >= 5)
            {
                if (m_highestBidSuit > (currSuit == null ? 0 : currSuit))
                {
                    return new Bid { Suit = m_highestBidSuit, Amount = (int)m_highestBid };
                }
            }

            return null;
        }

        public override int RequestDeclare()
        {
            int matchHighestBid = (from b in this.CurrentRoundStatus.Biddings
                                   where b != null
                                   orderby b.Value descending
                                   select b.Value.Amount).FirstOrDefault();

            m_highestBid = GetHighestBidForTrump(this.CurrentRoundStatus.Trump, this.Cards);

            return (int)m_highestBid;
        }

        #endregion
    }
}
