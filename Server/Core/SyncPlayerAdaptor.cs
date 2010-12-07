using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace Brain
{
    class SyncPlayerAdaptor : IAsyncPlayer
    {
        IPlayer imp;

        public SyncPlayerAdaptor(IPlayer _imp)
        {
            this.imp = _imp;
        }

        #region IHumanPlayer Members

        public void RequestBid()
        {
            Bid? b = this.imp.RequestBid();
            if( OnGetBidCompleted != null)
                OnGetBidCompleted(this, new RecieveBidEventArgs(b));
        }

        public void RequestDeclare()
        {
            int declare = this.imp.RequestDeclare();
            if (OnGetContractCompleted != null)
                OnGetContractCompleted(this, new RecieveContractEventArgs(declare));
        }

        public event EventHandler<RecieveBidEventArgs> OnGetBidCompleted;

        public event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;

        public void RequestExhangeCards()
        {
            Card[] cards = this.imp.RequestExhangeCards();
            if (OnGetExchangedCardsCompleted != null)
                OnGetExchangedCardsCompleted(this, new RecieveCardsEventArgs(cards));
        }

        public event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;

        public void RequestPlay()
        {
            Card play = this.imp.RequestPlay();
            if (OnGetPlayCompleted != null)
                OnGetPlayCompleted(this, new RecievePlayEventArgs(play));
        }

        public event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;

        #endregion

        #region IPlayerBase Members

        public void RecieveCards(Card[] cards)
        {
            this.imp.RecieveCards(cards);
        }

        public void RecieveExchangeCards(Card[] cards)
        {
            this.imp.RecieveExchangeCards(cards);
        }

        public void UpdateRoundStatus(RoundStatus status)
        {
            this.imp.UpdateRoundStatus(status);
        }

        public void UpdateGameStatus(GameStatus status)
        {
            this.imp.UpdateGameStatus(status);
        }

        public event EventHandler<EventArgs> OnUpdateStatusRequested
        {
            add { this.imp.OnUpdateStatusRequested += value; }
            remove { this.imp.OnUpdateStatusRequested -= value; }
        }

        public event EventHandler<EventArgs> OnKillGameRequested
        {
            add { this.imp.OnKillGameRequested += value; }
            remove { this.imp.OnKillGameRequested -= value; }
        }

        public string Name
        {
            get { return this.imp.Name; }
        }

        public void RecieveErrorMessage(string msg)
        {
            // nothing to do
        }

        #endregion
    }
}
