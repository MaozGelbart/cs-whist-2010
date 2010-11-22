using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;

namespace Brain
{
    public class GameFactory
    {
        public static List<Game> Games = new List<Game>();

        public static void CreateGame(IPlayer creator, int numberOfAIPlayers, string[] player_AI)
        {
            if (numberOfAIPlayers < 0 || numberOfAIPlayers > 4)
                return;
            Games = new List<Game>();
            Game g = new Game();
            g.AddPlayer(creator);
            for (int i = 0; i < numberOfAIPlayers; i++)
            {
                IPlayer p = Brain.PlayerFactory.CreatePlayer(player_AI[i]);
                if (p != null)
                    g.AddPlayer(p);
            }
            Games.Add(g);
        }

        public static void CreateGame(Server.Clients.ViewerSLAdaptor viewer, string[] plugins)
        {
            Games = new List<Game>();
            Game g = new Game();
            g.Viewer = viewer;
            Games.Add(g);
            for (int i = 0; i < plugins.Length; i++)
            {
                IPlayer p = Brain.PlayerFactory.CreatePlayer(plugins[i]);
                if (p != null)
                    g.AddPlayer(p);
            }
        }

        public static void AddPlayer(IPlayer player)
        {
            Game game = (from g in Games
                         where !g.IsStarted()
                         select g).FirstOrDefault();
            if (game == null)
            {
                Console.WriteLine("No opened games, player refused");
            }
            else
            {
                game.AddPlayer(player);
            }
        }

        internal static void KillGame(Game game)
        {
            Games.Remove(game);
        }
    }
}