using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;

namespace WCFServer.Clients
{
    public class PlayerSLAdaptor : IAsyncPlayer
    {

        public string SessionID { get; set; }

        private string _name;

        private IPlayerClient webClient;

        public PlayerSLAdaptor(string Name, string SessionID, IPlayerClient webClient)
        {
            this.SessionID = SessionID;
            this._name = Name;
            this.webClient = webClient;
        }

        #region IPlayer Members

        public void RecieveCards(Card[] cards)
        {
            this.webClient.RecieveCards(cards);
        }

        public void RecieveExchangeCards(Card[] cards)
        {
            this.webClient.RecieveExchangedCards(cards);
        }

        public void UpdateRoundStatus(RoundStatus status)
        {
            try
            {
                this.webClient.RecieveRoundStatusUpdate(status);
            }
            catch (Exception e)
            {
            }
        }

        public void UpdateGameStatus(GameStatus status)
        {
            try
            {
                this.webClient.RecieveGameStatusUpdate(status);
            }
            catch (Exception e)
            {
            }
        }

        public void RequestBid()
        {
            this.webClient.RequestBid();
        }

        public void RequestDeclare()
        {
            this.webClient.ReqeustContract();
        }

        public void RequestExhangeCards()
        {
            this.webClient.RequestExchangeCards();
        }

        public void RequestPlay()
        {
            this.webClient.RequestPlay();
        }

        public event EventHandler<RecieveBidEventArgs> OnGetBidCompleted;
        public void InvokeGetBidCompleted(Bid? b)
        {
            if (OnGetBidCompleted != null)
                OnGetBidCompleted(this, new RecieveBidEventArgs(b));
        }

        public event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;
        public void InvokeOnGetExchangedCardsCompleted(Card[] cards)
        {
            if (OnGetExchangedCardsCompleted != null)
                OnGetExchangedCardsCompleted(this, new RecieveCardsEventArgs(cards));
        }


        public event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;
        public void InvokeOnGetContractCompleted(int b)
        {
            if (OnGetContractCompleted != null)
                OnGetContractCompleted(this, new RecieveContractEventArgs(b));
        }


        public event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;
        public void InvokeOnGetPlayCompleted(Card play)
        {
            if (OnGetPlayCompleted != null)
                OnGetPlayCompleted(this, new RecievePlayEventArgs(play));
        }


        public event EventHandler<EventArgs> OnUpdateStatusRequested;
        public void InvokeOnUpdateStatusRequested()
        {
            if (OnUpdateStatusRequested != null)
                OnUpdateStatusRequested(this, new EventArgs());
        }

        public event EventHandler<EventArgs> OnKillGameRequested;
        public void KillGame()
        {
            if (OnKillGameRequested != null)
                OnKillGameRequested(this, new EventArgs());
        }

        public string Name
        {
            get { return _name; }
        }

        public void RecieveErrorMessage(string msg)
        {
            try
            {
                this.webClient.RecieveErrorMessage(msg);
            }
            catch { }
        }

        public void RecieveChatMessage(PlayerSeat sender, string msg)
        {
            try
            {
                this.webClient.RecieveChatMessage(sender, msg);
            }
            catch { }
        }

        public event EventHandler<RecieveChatMessageEventArgs> OnSendChatMessage;
        public void SendMessage(string msg)
        {
            if (OnSendChatMessage != null)
                OnSendChatMessage(this, new RecieveChatMessageEventArgs(msg));
        }


        #endregion
    }
}