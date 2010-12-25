using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Inherit this class to implement gamer part
    /// </summary>
    public abstract class GamerBase : PlayerPartExtender, IGamer
    {
        public abstract Card RequestPlay();


        protected ISet<int>[] m_playedCards;
        protected ISet<Suit?>[] m_playerEmptySuits;

        /// <summary>
        /// Memory related function which updates player's memory to have currently played cards & player's empty suites
        /// </summary>
        /// <returns></returns>
        public void UpdateMemory(ISet<int>[] playedCards, ISet<Suit?>[] playerEmptySuits)
        { 
            this.m_playedCards      = playedCards;
            this.m_playerEmptySuits = playerEmptySuits;
        }

        /// <summary>
        /// return true if player p got more cards of asked suit
        /// return false if player p have no more cards of asked suit
        /// </summary>
        /// <param name="p">asked player</param>
        /// <param name="s">asked suit</param>
        /// <returns></returns>
        protected bool getPlayerSuitStatus(PlayerSeat p, Suit s)
        {
            return !m_playerEmptySuits[(int)p - 1].Contains(s);
        }

        /// <summary>
        /// Returns statistics of a player having an asked card (0 means - no chance)
        /// </summary>
        /// <param name="player">asked player</param>
        /// <param name="card">asked card</param>
        /// <returns></returns>
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
            for (int i = (int)CurrentRoundStatus.LeadingPlayer; i < 4; i++)
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

            //card was not thrown yet. calculate statistics (acctually left!=0 otherwise we already returned... but who cares...)
            int left = (13 - m_playedCards[(int)card.Suit - 1].Count);
            double retVal = (left == 0) ? 0 : 1/left;

            return retVal;
        }
    }
}
