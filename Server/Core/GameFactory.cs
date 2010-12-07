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

        public static void CreateGame(IAsyncPlayer creator, int numberOfAIPlayers, string[] player_AI, int num_of_rounds, int milisecs_between_turns)
        {
            if (numberOfAIPlayers < 0 || numberOfAIPlayers > 4)
                return;
            Games = new List<Game>();
            Game g = new Game(num_of_rounds, milisecs_between_turns);
            g.AddPlayer(creator);
            for (int i = 0; i < numberOfAIPlayers; i++)
            {
                IAsyncPlayer p = Brain.PlayerFactory.CreatePlayer(player_AI[i]);
                if (p != null)
                    g.AddPlayer(p);
            }
            Games.Add(g);
        }

        public static void CreateGame(IGameViewer viewer, string[] plugins, int num_of_rounds, int milisecs_between_turns)
        {
            Games = new List<Game>();
            Game g = new Game(num_of_rounds, milisecs_between_turns);
            g.Viewer = viewer;
            Games.Add(g);
            for (int i = 0; i < plugins.Length; i++)
            {
                IAsyncPlayer p = Brain.PlayerFactory.CreatePlayer(plugins[i]);
                if (p != null)
                    g.AddPlayer(p);
            }
        }

        public static bool AddPlayer(IAsyncPlayer player)
        {
            Game game = (from g in Games
                         where !g.IsStarted()
                         select g).FirstOrDefault();
            if (game == null)
            {
                Console.WriteLine("No opened games, player refused");
                player.RecieveErrorMessage("No opened games, player refused");
                return false;
            }
            else
            {
                game.AddPlayer(player);
                return true;
            }
        }

        internal static void KillGame(Game game)
        {
            Games.Remove(game);
        }
    }
}