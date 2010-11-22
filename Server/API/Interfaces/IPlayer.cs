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
    public interface IPlayer : IPlayerBase, ICardExchanger, IBidder, IPlayerGamer
    {
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
   
}
