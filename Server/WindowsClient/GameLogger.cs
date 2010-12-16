using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;
using System.IO;

namespace WindowsClient
{
    class GameLogger : IGameViewer
    {
        string logfile;
        StreamWriter writer;
        public GameLogger(string logfile)
        {
            this.logfile = logfile;
            writer = new StreamWriter(logfile);
            writer.WriteLine("starting log");
        }

        #region IGameViewer Members

        public void UpdateGameStatus(GameStatus status)
        {
            writer.WriteLine("Round no. " + status.RoundNumber);
            writer.WriteLine("Scores:");
            writer.WriteLine(status.PlayerNames.Aggregate((s, acm) => acm += "/t" + s));
            writer.WriteLine(status.Scores.Aggregate<int,string>("", (acm, score) => acm += "/t" + score));

        }

        public void UpdateRoundStatus(RoundStatus status, Card[][] allCards)
        {
        }

        public event EventHandler<EventArgs> OnKillGameRequested;

        public void RecieveErrorMessage(string msg)
        {
            writer.WriteLine("Game over");
            writer.Flush();
            writer.Close();
        }


        public void RecieveGameOver()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
