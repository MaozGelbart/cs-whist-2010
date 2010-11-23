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
        }

        public virtual void RecieveExchangeCards(Card[] cards)
        {
            Cards.AddRange(cards);
        }

        public virtual void UpdateRoundStatus(RoundStatus status)
        {
            CurrentRoundStatus = status;
        }

        public virtual void UpdateGameStatus(GameStatus status)
        {
            CurrentGameStatus = status;
        }

        public event EventHandler<EventArgs> OnUpdateStatusRequested;

        public event EventHandler<EventArgs> OnKillGameRequested;

        public abstract string Name { get; }

        #endregion
    }
}
