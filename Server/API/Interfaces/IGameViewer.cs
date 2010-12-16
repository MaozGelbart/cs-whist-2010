using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    public interface IGameViewer
    {
        void UpdateGameStatus(GameStatus status);

        void UpdateRoundStatus(RoundStatus status, Card[][] allCards);

        event EventHandler<EventArgs> OnKillGameRequested;

        void RecieveErrorMessage(string msg);

        void RecieveGameOver();
    }
}
