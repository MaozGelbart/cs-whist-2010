using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;

namespace PlugIn
{
    public class DumbPlayer : IPlayer
    {
        #region IPlayer Members

        protected List<Card> cards;
        protected RoundStatus status;
        protected GameStatus gameStatus;
        protected Card? cardToThrow = null;

        public void RecieveCards(Card[] cards)
        {
            this.cards = new List<Card>(cards);
        }

        public void RecieveExchangeCards(Card[] cards)
        {
            this.cards.AddRange(cards);
        }

        public void UpdateRoundStatus(RoundStatus status)
        {
            this.status = status;
            if (cardToThrow != null)
            {
                cards.Remove(cardToThrow.Value);
                cardToThrow = null;
            }
        }

        public void UpdateGameStatus(GameStatus status)
        {
            this.gameStatus = status;
        }

        public virtual Bid? RequestBid()
        {
            return null;
        }

        public virtual int RequestDeclare()
        {
            Random rnd = new Random();
            return rnd.Next(3) + 1;
        }

        public Card[] RequestExhangeCards()
        {
            List<Card> given = cards.GetRange(0, 3);
            cards.RemoveRange(0, 3);
            return given.ToArray();
        }

        public Card RequestPlay()
        {
            Card? card = null;
            if (status.LeadingPlayer > 0)
            {
                Suit s = status.CurrentPlay[(int)status.LeadingPlayer].Value.Suit;
                card = (from cr in cards
                          where cr.Suit == s
                          select cr).FirstOrDefault();
            }
            if (card == null || card.Value.Value == 0)
            {
                card = cards[0];
            }
          //  cards.Remove(card.Value);
            cardToThrow = card;
            return card.Value;
        }

        public event EventHandler<EventArgs> OnUpdateStatusRequested;
        private void InvokeOnUpdateStatusRequested()
        {
            if (OnUpdateStatusRequested != null)
                OnUpdateStatusRequested(this, new EventArgs());
        }

        public event EventHandler<EventArgs> OnKillGameRequested;

        public virtual string Name
        {
            get { return "Dumb"; }
        }


        #endregion
    }
}