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

        protected List<Card>[] ArrangeCardBySuits(ICollection<Card> cards)
        {
            List<Card>[] retval = new List<Card>[4];
            for (int i = 0; i < 4; i++)
            {
                retval[i] = new List<Card>();
            }

            foreach (Card c in cards)
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
            List<Card>[] cardsBySuit = ArrangeCardBySuits(cards);
            double totalBid = 0;
            int number_of_unused_trumps;
            totalBid += CalcBidForStrongTrump(cardsBySuit[(int)suit - 1], out number_of_unused_trumps);
            for (int i = 0; i < cardsBySuit.Length; i++)
            {
                List<Card> currList = cardsBySuit[i];

                // this is the list of the 'strong' trump
                if ((i + 1) != (int)suit)
                {
                    totalBid += CalcBidForWeakTrump(currList, ref number_of_unused_trumps);
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
        private double CalcBidForStrongTrump(ICollection<Card> cards, out int number_of_unused_trumps)
        {
            double totalBid = 0;
            number_of_unused_trumps = cards.Count;
            int number_of_strong_trumps_i_dont_have = 0;
            for (int current_card = 14; current_card > 1; current_card--)
            {
                if (ContainsValue(cards, current_card))
                {
                    // if the number of trumps I have (beside the current one) is enough (heruistically) to make other players
                    // secrefice their stronger cards
                    if (number_of_strong_trumps_i_dont_have > number_of_unused_trumps - 1 )
                    {
                        return totalBid;
                    }
                    else
                    {
                        // substract the amount of trumps needed to secrefice until the current is the highest one
                        number_of_unused_trumps -= number_of_strong_trumps_i_dont_have;
                        // substruct the current one
                        number_of_unused_trumps--;
                        // all stronger cards were cleared
                        number_of_strong_trumps_i_dont_have = 0;
                        totalBid++;
                    }
                }
                else
                    number_of_strong_trumps_i_dont_have++;
            }
            return totalBid;
        }

        /// <summary>
        /// Calculates the best bid for a set of cards of the same
        /// suit that is not the 'strong' trump.
        /// </summary>
        private double CalcBidForWeakTrump(ICollection<Card> cards, ref int number_of_unused_trumps)
        {
            double totalBid = 0;

            //check if has A
            if (ContainsValue(cards, 14))
            {
                totalBid++;
            }

            //check if has K & 1 more from suit
            if (ContainsValue(cards, 13) && cards.Count >= 2)
            {
                totalBid++;
            }

            //check if has Q & 2 more from suit
            if (ContainsValue(cards, 12) && cards.Count >= 3)
            {
                totalBid+= (2.0/3);
            }

            //no cards from suit
            if (cards.Count == 0)
            {
                //TODO: not sure!!!! read http://www.myspades.com/bidding-strategy.php and fix!!!!
                double to_add = Math.Min(1.5, (double)number_of_unused_trumps / 1.5);
                totalBid += to_add;
                number_of_unused_trumps -= (int)Math.Ceiling(to_add * 1.5);
            }

            //1 card from suit
            if (cards.Count == 1)
            {
                //TODO: not sure!!!! read http://www.myspades.com/bidding-strategy.php and fix!!!!
                double to_add = Math.Min(1, number_of_unused_trumps / 2.5);
                totalBid += to_add;
                number_of_unused_trumps -= (int)(to_add * 2.5);
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
