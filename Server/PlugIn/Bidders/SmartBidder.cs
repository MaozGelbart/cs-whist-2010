﻿using System;
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

            // bid is ICompareable !! this means you can sort by simply the bid rather than implement comparing
                Bid? currHighestBidObj = (from b in this.CurrentRoundStatus.Biddings
                                          where b != null
                                          orderby b.Value descending
                                          select b.Value).FirstOrDefault();

                Bid myHighestBid = new Bid { Amount = (int)m_highestBid, Suit = m_highestBidSuit };


            // each bidding round the minimum grows by one : 5 , 6 , 7
            if (myHighestBid.Amount >= (5 + this.CurrentRoundStatus.TurnNumber))
            {
                // if there is no heighest bid or my bid is heigher than the highest one
                if( !currHighestBidObj.HasValue || myHighestBid > currHighestBidObj.Value)
                {
                    return myHighestBid;
                }
            }

            return null;
        }

        public override int RequestDeclare()
        {
            /*  unused
            int matchHighestBid = (from b in this.CurrentRoundStatus.Biddings
                                   where b != null
                                   orderby b.Value descending
                                   select b.Value.Amount).FirstOrDefault();
            */
            m_highestBid = GetHighestBidForTrump(this.CurrentRoundStatus.Trump, this.Cards);

            return (int)m_highestBid;
        }

        #endregion
    }
}
