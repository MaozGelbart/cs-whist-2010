using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;
using WCFServer;

namespace Server.Clients
{
    public class ViewerSLAdaptor : IGameViewer
    {
        public string SessionID { get; set; }
        IPlayerClient webClient;

        public ViewerSLAdaptor(string SessionID, IPlayerClient webClient)
        {
            this.SessionID = SessionID;
            this.webClient = webClient;
        }

        public void UpdateGameStatus(GameStatus status)
        {
            try
            {
                webClient.RecieveGameStatusUpdate(status);
            }
            catch (Exception e)
            {
            }
        }

        public void UpdateRoundStatus(RoundStatus status, Card[][] allCards)
        {
            try
            {
                webClient.RecieveStatusCards(status, allCards);
            }
            catch (Exception e)
            {
            }
        }

        internal void KillGame()
        {
            if (OnKillGameRequested != null)
                OnKillGameRequested(this, new EventArgs());
        }

        public event EventHandler<EventArgs> OnKillGameRequested;

    
        public void  RecieveErrorMessage(string msg)
        {
            try
            {
                webClient.RecieveErrorMessage(msg);
            }
            catch (Exception e)
            {
            }
        }

        public void RecieveGameOver()
        {
            try
            {
                webClient.RecieveGameOver();
            }
            catch (Exception e)
            {
            }
        }
    }
}