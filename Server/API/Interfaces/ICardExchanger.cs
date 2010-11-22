using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Handles only the frishing phase
    /// </summary>
    public interface ICardExchanger
    {
        /// <summary>
        /// Request for a 3 cards exchange, eventually expected to invoke OnGetExchangedCardsCompleted
        /// </summary>
        void RequestExhangeCards();

        event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;

    }
}
