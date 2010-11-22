using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn
{
    public class ExternalPlayer : IPlayer
    {
        #region IPlayer Members

        public void RecieveCards(Card[] cards)
        {
        }

        public void RecieveExchangeCards(Card[] cards)
        {
        }

        public void UpdateRoundStatus(RoundStatus status)
        {
        }

        public void UpdateGameStatus(GameStatus status)
        {
        }

        public void RequestBid()
        {
        }

        public void RequestDeclare()
        {
        }

        public void RequestExhangeCards()
        {
        }

        public void RequestPlay()
        {
        }

        public event EventHandler<RecieveBidEventArgs> OnGetBidCompleted;

        public event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;

        public event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;

        public event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;

        public event EventHandler<EventArgs> OnUpdateStatusRequested;

        public event EventHandler<EventArgs> OnKillGameRequested;

        public string Name
        {
            get { return "X"; }
        }

        #endregion
    }
}
