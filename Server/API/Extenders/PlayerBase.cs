using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    public abstract class PlayerBase : IPlayerBase
    {
        #region Protected Members

        protected virtual RoundStatus CurrentRoundStatus { get; set; }

        protected virtual GameStatus CurrentGameStatus { get; set; }

        protected virtual List<Card> Cards { get; set; }

        protected virtual void PassCards(Card card1, Card card2, Card card3)
        {
            Cards.Remove(card1);
            Cards.Remove(card2);
            Cards.Remove(card3);
        }

        protected virtual void ThrowCard(Card card)
        {
            Cards.Remove(card);
        }

        #endregion

        #region IPlayerBase Members

        public virtual void RecieveCards(Card[] cards)
        {
            Cards = new List<Card>(cards);

            //reset memory & empty suits
            m_playedCards = new HashSet<int>[4];
            for (int i = 0; i < 4; i++)
            {
                m_playedCards[i] = new HashSet<int>();
            }
            
            m_playerEmptySuits = new HashSet<Suit?>[3];
            for (int i = 0; i < 3; i++)
            {
                m_playerEmptySuits[i] = new HashSet<Suit?>();
            }
        }

        public virtual void RecieveExchangeCards(Card[] cards)
        {
            Cards.AddRange(cards);
        }

        public virtual void UpdateRoundStatus(RoundStatus status)
        {
            CurrentRoundStatus = status;  

            //end of play?
            if (status.State == RoundState.Results)
            {
                calculateRound(status);
            }
        }

        public virtual void UpdateGameStatus(GameStatus status)
        {
            CurrentGameStatus = status;
        }

        public event EventHandler<EventArgs> OnUpdateStatusRequested;

        public event EventHandler<EventArgs> OnKillGameRequested;

        public abstract string Name { get; }

        #endregion

        #region IPlayerBase Memory

        protected ISet<int>[] m_playedCards;
        protected ISet<Suit?>[] m_playerEmptySuits;

        private void calculateRound(RoundStatus status)
        {
            //check all other players
            for (int i = 0; i < 3; i++)
            {
                //different suit?
                if (status.getCurrentPlaySuit() != status.GetCurrentPlay((PlayerSeat)i + 1).Value.Suit)
                {
                    m_playerEmptySuits[i].Add(status.getCurrentPlaySuit());
                }
            }

            //update memory
            updateMemory(status.CurrentPlay);

            double d = getCardStatistic((PlayerSeat)2, new Card(Suit.Hearts, 12));
            d = getCardStatistic((PlayerSeat)2, new Card(Suit.Hearts, 3));
            d = getCardStatistic((PlayerSeat)2, new Card(Suit.Clubs, 5));
        }

        /// <summary>
        /// happens at the end of each round - insert played cards to memory
        /// </summary>
        /// <param name="roundCards">played cards</param>
        private void updateMemory(Card?[] roundCards)
        {
            foreach (Card c in roundCards)
            {
                m_playedCards[(int)c.Suit - 1].Add(c.Value);
            }
        }

        protected bool getPlayerSuitStatus(PlayerSeat p, Suit s)
        { 
            return !m_playerEmptySuits[(int)p].Contains(s);
        }

        protected double getCardStatistic(PlayerSeat player, Card card)
        {
            //check if player have suit?
            if (!getPlayerSuitStatus(player, card.Suit))
            {
                return 0;
            }

            //check if card was thrown before?
            if (m_playedCards[(int)card.Suit - 1].Contains(card.Value))
            {
                return 0;
            }

            //check if card was played in this play?
            for (int i = (int)CurrentRoundStatus.LeadingPlayer ; i < 4 ; i++)
            {
                Card? tmp = CurrentRoundStatus.CurrentPlay[i % 4];
                if (tmp != null)
                {
                    m_playedCards[(int)tmp.Value.Suit - 1].Add(tmp.Value.Value);

                    if (CurrentRoundStatus.CurrentPlay[i % 4].Equals(card))
                    {
                        return 0;
                    }
                }
                else
                {
                    break;
                }
            }

            //card was not thrown yet. calculate statistics
            double retVal = (13 - m_playedCards[(int)card.Suit - 1].Count) / 13;

            return retVal;
        }
        #endregion
    }
}
