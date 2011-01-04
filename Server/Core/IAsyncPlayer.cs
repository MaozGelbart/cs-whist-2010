using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// The main interface must be implemented to create a humen or AI player.
    /// MUST HAVE AN ARGUMENT-LESS CONSTRUCTOR if to be created from PlayerFactory
    /// </summary>
    public interface IAsyncPlayer : IPlayerBase
    {
        /// <summary>
        /// Request for a bid, eventually expected to invoke OnGetBidCompleted
        /// </summary>
        void RequestBid();

        /// <summary>
        /// Request for a contract, eventually expected to invoke OnGetContractCompleted
        /// </summary>
        void RequestDeclare();

        event EventHandler<RecieveBidEventArgs> OnGetBidCompleted;

        event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;

        
        /// <summary>
        /// Request for a 3 cards exchange, eventually expected to invoke OnGetExchangedCardsCompleted
        /// </summary>
        void RequestExhangeCards();

        event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;

        /// <summary>
        /// Request for a card to throw, eventually expected to invoke OnGetPlayCompleted
        /// </summary>
        void RequestPlay();

        event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;

        /// <summary>
        /// Mainly uses for human players
        /// </summary>
        /// <param name="msg"></param>
        void RecieveErrorMessage(string msg);

        /// <summary>
        /// Uses for human players, to chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        void RecieveChatMessage(PlayerSeat sender, string msg);

        event EventHandler<RecieveChatMessageEventArgs> OnSendChatMessage;

        string PhotoUrl { get; }
    }

    /// <summary>
    /// Callback result for bid request
    /// </summary>
    public class RecieveBidEventArgs : EventArgs
    {
        public Bid? Bid { get; set; }

        public RecieveBidEventArgs(Bid? bid)
            :base()
        {
            this.Bid = bid;
        }
    }

    /// <summary>
    /// Callback result for contract request
    /// </summary>
    public class RecieveContractEventArgs : EventArgs
    {
        public int Amount { get; set; }
        public RecieveContractEventArgs(int amount)
        {
            this.Amount = amount;
        }
    }

    /// <summary>
    /// Callback result for card exchange request, this.Cards should be a 3 elements array
    /// </summary>
    public class RecieveCardsEventArgs : EventArgs
    {
        public Card[] Cards { get; set; }

        public RecieveCardsEventArgs(Card[] cards)
            :base()
        {
            this.Cards = cards;
        }
    }

    /// <summary>
    /// Callback result for play request
    /// </summary>
    public class RecievePlayEventArgs : EventArgs
    {
        public Card Play { get; set; }
        public RecievePlayEventArgs(Card play)
            :base()
        {
            this.Play = play;
        }
    }

    /// <summary>
    /// Callback result for chatting
    /// </summary>
    public class RecieveChatMessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public RecieveChatMessageEventArgs(string msg)
            :base()
        {
            this.Message = msg;
        }
    }
   
}
