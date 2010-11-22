using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;

namespace Server.Clients
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

        public virtual void RequestBid()
        {
            InvokeGetBidCompleted(null);
        }

        public virtual void RequestDeclare()
        {
            Random rnd = new Random();
            InvokeOnGetContractCompleted(rnd.Next(3) + 1);
        }

        public void RequestExhangeCards()
        {
            List<Card> given = cards.GetRange(0, 3);
            cards.RemoveRange(0, 3);
            InvokeOnGetExchangedCardsCompleted(given.ToArray());
        }

        public void RequestPlay()
        {
            Card? card = null;
            if (status.LeadingPlayer > 0)
            {
                Suit s = status.CurrentPlay[status.LeadingPlayer].Value.Suit;
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
            this.InvokeOnGetPlayCompleted(card.Value);
        }

        public event EventHandler<RecieveBidEventArgs> OnGetBidCompleted;
        protected void InvokeGetBidCompleted(Bid? b)
        {
            if (OnGetBidCompleted != null)
                OnGetBidCompleted(this, new RecieveBidEventArgs(b));
        }

        public event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;
        private void InvokeOnGetExchangedCardsCompleted(Card[] cards)
        {
            if (OnGetExchangedCardsCompleted != null)
                OnGetExchangedCardsCompleted(this, new RecieveCardsEventArgs(cards));
        }


        public event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;
        protected void InvokeOnGetContractCompleted(int b)
        {
            if (OnGetContractCompleted != null)
                OnGetContractCompleted(this, new RecieveContractEventArgs(b));
        }


        public event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;
        private void InvokeOnGetPlayCompleted(Card play)
        {
            if (OnGetPlayCompleted != null)
                OnGetPlayCompleted(this, new RecievePlayEventArgs(play));
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