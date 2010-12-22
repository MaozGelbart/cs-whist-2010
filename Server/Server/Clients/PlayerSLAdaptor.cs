using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;
using Brain;
using System.Threading;

namespace WCFServer.Clients
{
    public class PlayerSLAdaptor : IAsyncPlayer
    {

        #region Constructor

        public PlayerSLAdaptor(string Name, string SessionID, IPlayerClient webClient)
        {
            this.SessionID = SessionID;
            this._name = Name;
            this.webClient = webClient;
            ResponseRecieved();
        }

        #endregion

        #region Private Members

        private string _name;

        private IPlayerClient webClient;

        private DateTime plannedPlayerDeathTime;

        private Timer playerKiller;

        /// <summary>
        /// Every player has a time of death planned unless he does actions, to prevent saving clients on memory of the
        /// server for nothing
        /// </summary>
        private void ResponseRecieved()
        {
            plannedPlayerDeathTime = DateTime.Now.Add(Game.TIME_TO_CALL_GAME_DEATH);
            if (playerKiller == null)
            {
                playerKiller = new Timer(
                    delegate(object obj)
                    {
                        if (plannedPlayerDeathTime < DateTime.Now)
                        {
                            PlayerService.KillPlayer(this);
                            playerKiller.Dispose();
                        }
                    }
                    ,null
                    , Game.TIME_TO_CALL_GAME_DEATH,
                    Game.TIME_TO_CALL_GAME_DEATH);
            }
        }

        #endregion

        #region Public Members

        public string SessionID { get; set; }

        #endregion

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
            ResponseRecieved();
            if (OnGetBidCompleted != null)
                OnGetBidCompleted(this, new RecieveBidEventArgs(b));
        }

        public event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;
        public void InvokeOnGetExchangedCardsCompleted(Card[] cards)
        {
            ResponseRecieved();
            if (OnGetExchangedCardsCompleted != null)
                OnGetExchangedCardsCompleted(this, new RecieveCardsEventArgs(cards));
        }


        public event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;
        public void InvokeOnGetContractCompleted(int b)
        {
            ResponseRecieved();
            if (OnGetContractCompleted != null)
                OnGetContractCompleted(this, new RecieveContractEventArgs(b));
        }


        public event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;
        public void InvokeOnGetPlayCompleted(Card play)
        {
            ResponseRecieved();
            if (OnGetPlayCompleted != null)
                OnGetPlayCompleted(this, new RecievePlayEventArgs(play));
        }


        public event EventHandler<EventArgs> OnUpdateStatusRequested;
        public void InvokeOnUpdateStatusRequested()
        {
            ResponseRecieved();
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
            ResponseRecieved();
            if (OnSendChatMessage != null)
                OnSendChatMessage(this, new RecieveChatMessageEventArgs(msg));
        }


        #endregion
    }
}