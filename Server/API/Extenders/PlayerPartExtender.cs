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
            cards.Remove(highest);
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
            cards.Remove(lowest);
            return lowest;
        }

        #endregion
    }
}
