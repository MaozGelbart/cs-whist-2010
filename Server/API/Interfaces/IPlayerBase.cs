using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Handles all basic player communication with Brain
    /// </summary>
    public interface IPlayerBase
    {
        void RecieveCards(Card[] cards);

        void RecieveExchangeCards(Card[] cards);

        void UpdateRoundStatus(RoundStatus status);

        void UpdateGameStatus(GameStatus status);

        /// <summary>
        /// Invoke this event to demand a status update
        /// </summary>
        event EventHandler<EventArgs> OnUpdateStatusRequested;

        /// <summary>
        /// Invoke this event to kill the current game
        /// </summary>
        event EventHandler<EventArgs> OnKillGameRequested;

        /// <summary>
        /// Player name as it would appear to users
        /// </summary>
        string Name { get; }
    }
}
