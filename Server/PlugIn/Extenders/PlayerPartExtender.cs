using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Helper class, inherit this class to implement a part of player (bidder,card-exchanger, gamer).
    /// Use statuses and cards properties only for reading, is recommended.
    /// </summary>
    /// <typeparam name="IT"></typeparam>
    public abstract class PlayerPartExtender
    {

        public RoundStatus CurrentRoundStatus
        {
            get;
            set;
        }

        public GameStatus CurrentGameStatus
        {
            get;
            set;
        }

        public List<Card> Cards
        {
            get;
            set;
        }

        private Suit m_highestBidSuit;

        #region Helpers Methods

        protected List<Card>[] ArrangeCardBySuits()
        {
            List<Card>[] retval = new List<Card>[4];
            for (int i = 0; i < 4; i++)
            {
                retval[i] = new List<Card>();
            }

            foreach (Card c in Cards)
            {
                retval[(int)c.Suit - 1].Add(c);
            }

            return retval;
        }

        protected Card GetHighestCardInCollection(ICollection<Card> cards)
        {
            Card highest = cards.First();
            foreach (Card c in cards)
            {
                if (highest.Value < c.Value)
                {
                    highest = c;
                }
            }
            return highest;
        }

        protected Card GetLowestCardInCollection(ICollection<Card> cards)
        {
            Card lowest = cards.First();
            foreach (Card c in cards)
            {
                if (lowest.Value > c.Value)
                {
                    lowest = c;
                }
            }
            return lowest;
        }

        #endregion

        #region Score Related Helper Methods

        /// <summary>
        /// Returns the best bid for a specific 'strong' trump. 
        /// </summary>
        protected double GetHighestBidForTrump(Suit? suit, ICollection<Card> cards)
        {
            List<Card>[] cardsBySuit = ArrangeCardBySuits();
            double totalBid = 0;

            for (int i = 0; i < cardsBySuit.Length; i++)
            {
                List<Card> currList = cardsBySuit[i];

                // this is the list of the 'strong' trump
                if ((i + 1) == (int)suit)
                {
                    totalBid += CalcBidForStrongTrump(cards);
                }
                else
                {
                    totalBid += CalcBidForWeakTrump(cards);
                }
            }

            return totalBid;
        }

        /// <summary>
        /// Checks if a collection of cards of the same suit contains
        /// a specific value.
        /// </summary>
        private bool ContainsValue(ICollection<Card> cards, int value)
        {
            foreach (Card card in cards)
            {
                if (card.Value == value)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates the best bid for a set of same suit cards that
        /// are the 'strong' trump.
        /// </summary>
        private double CalcBidForStrongTrump(ICollection<Card> cards)
        {
            double totalBid = 0;

            if (ContainsValue(cards, 1))
            {
                if (ContainsValue(cards, 12) && cards.Count >= 3)
                {
                    totalBid += 2;
                }

                totalBid++;
            }

            if (ContainsValue(cards, 12) && !ContainsValue(cards, 1) && cards.Count >= 3)
            {
                totalBid++;
            }

            if (ContainsValue(cards, 13) && cards.Count >= 2)
            {
                if (ContainsValue(cards, 11) && cards.Count >= 4)
                {
                    totalBid += 2;
                }

                totalBid++;
            }

            if (cards.Count >= 4)
            {
                totalBid++;
                totalBid += (cards.Count - 4) * 0.5;
            }

            return totalBid;
        }

        /// <summary>
        /// Calculates the best bid for a set of cards of the same
        /// suit that is not the 'strong' trump.
        /// </summary>
        private double CalcBidForWeakTrump(ICollection<Card> cards)
        {
            double totalBid = 0;

            if (ContainsValue(cards, 1))
            {
                totalBid++;
            }

            if (ContainsValue(cards, 13) && cards.Count >= 2)
            {
                totalBid++;
            }

            if (ContainsValue(cards, 12) && cards.Count >= 3)
            {
                totalBid++;
            }

            if (cards.Count == 0)
            {
                totalBid -= 1.5;
            }

            if (cards.Count == 1)
            {
                totalBid -= 1;
            }

            return totalBid;
        }

        /// <summary>
        /// Returns the best bid out of all possible trump choices
        /// for a set of cards. 
        /// </summary>
        protected double GetHighestBid(ICollection<Card> cards)
        {
            double maxBid = 0;
            double bestBidForTrump = 0;

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                bestBidForTrump = GetHighestBidForTrump(suit, cards);

                if (bestBidForTrump > maxBid)
                {
                    maxBid = bestBidForTrump;
                    m_highestBidSuit = suit;
                }
            }

            return maxBid;
        }

        protected Suit GetHighestBidSuit()
        {
            return m_highestBidSuit;
        }

        #endregion
    
    }
}
