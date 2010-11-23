using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Handles only the card throwing game rounds
    /// </summary>
    public interface IGamer
    {
        /// <summary>
        /// Request for a card to throw, eventually expected to invoke OnGetPlayCompleted
        /// </summary>
        Card RequestPlay();

    }
}
