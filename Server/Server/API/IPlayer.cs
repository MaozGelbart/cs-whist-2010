using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    public interface IPlayer
    {
        void RecieveCards(Card[] cards);

        void RecieveExchangeCards(Card[] cards);

        void UpdateRoundStatus(RoundStatus status);

        void UpdateGameStatus(GameStatus status);

        void RequestBid();

        void RequestDeclare();

        void RequestExhangeCards();

        void RequestPlay();

        event EventHandler<RecieveBidEventArgs> OnGetBidCompleted;

        event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;

        event EventHandler<RecieveContractEventArgs> OnGetContractCompleted;

        event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;

        event EventHandler<EventArgs> OnUpdateStatusRequested;

        string Name { get; }
    }

    public class RecieveBidEventArgs : EventArgs
    {
        public Bid? Bid { get; set; }

        public RecieveBidEventArgs(Bid? bid)
            :base()
        {
            this.Bid = bid;
        }
    }

    public class RecieveContractEventArgs : EventArgs
    {
        public int Amount { get; set; }
        public RecieveContractEventArgs(int amount)
        {
            this.Amount = amount;
        }
    }

    public class RecieveCardsEventArgs : EventArgs
    {
        public Card[] Cards { get; set; }

        public RecieveCardsEventArgs(Card[] cards)
            :base()
        {
            this.Cards = cards;
        }
    }

    public class RecievePlayEventArgs : EventArgs
    {
        public Card Play { get; set; }
        public RecievePlayEventArgs(Card play)
            :base()
        {
            this.Play = play;
        }
    }
   
}
