using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Handles only the bidding and contract declaring phase
    /// </summary>
    public interface IBidder
    {
        /// <summary>
        /// Request for a bid, eventually expected to invoke OnGetBidCompleted
        /// </summary>
        Bid? RequestBid();

        /// <summary>
        /// Request for a contract, eventually expected to invoke OnGetContractCompleted
        /// </summary>
        int RequestDeclare();

    }
}
