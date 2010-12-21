using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;

namespace Brain
{
    /// <summary>
    /// Creates and kills games
    /// </summary>
    public class GameFactory
    {
        /// <summary>
        /// List of opened games
        /// </summary>
        public static List<Game> Games = new List<Game>();

        /// <summary>
        /// Creates a new game
        /// </summary>
        /// <param name="creator">Creator player ( uselly human)</param>
        /// <param name="numberOfAIPlayers">number of AI players (up to 3)</param>
        /// <param name="player_AI">strings representing AI players types</param>
        /// <param name="num_of_rounds">numebr of rounds to play</param>
        /// <param name="milisecs_between_turns">time to wait between each turn</param>
        /// <param name="game_name">name of game</param>
        public static void CreateGame(IAsyncPlayer creator, int numberOfAIPlayers, string[] player_AI, int num_of_rounds, int milisecs_between_turns, string game_name)
        {
            if (numberOfAIPlayers < 0 || numberOfAIPlayers > 4)
                return;
            if (game_name == null)
                game_name = "";
            Game g = new Game(num_of_rounds, milisecs_between_turns, game_name);
            g.AddPlayer(creator);
            for (int i = 0; i < numberOfAIPlayers; i++)
            {
                IAsyncPlayer p = Brain.PlayerFactory.CreatePlayer(player_AI[i]);
                if (p != null)
                    g.AddPlayer(p, player_AI[i]);
            }
            Games.Add(g);
        }

        /// <summary>
        /// Creates an all computer players game
        /// </summary>
        /// <param name="viewer">Object that receive all the information about the game real-time</param>
        /// <param name="plugins">array of 4 player types</param>
        /// <param name="num_of_rounds">number of rounds to play</param>
        /// <param name="milisecs_between_turns">time to wait between each turn</param>
        /// <param name="game_name">name of game</param>
        public static void CreateGame(IGameViewer viewer, string[] plugins, int num_of_rounds, int milisecs_between_turns, string game_name)
        {
            if (game_name == null)
                game_name = "";
            Game g = new Game(num_of_rounds, milisecs_between_turns, game_name);
            g.Viewer = viewer;
            Games.Add(g);
            for (int i = 0; i < plugins.Length; i++)
            {
                IAsyncPlayer p = Brain.PlayerFactory.CreatePlayer(plugins[i]);
                if (p != null)
                    g.AddPlayer(p, plugins[i]);
            }
        }

        /// <summary>
        /// Add player to an existing game (searches for games that haven't started yet)
        /// </summary>
        /// <param name="player">player to add</param>
        /// <param name="game_name">name of game</param>
        /// <returns></returns>
        public static bool AddPlayer(IAsyncPlayer player, string game_name)
        {
            if (game_name == null)
                game_name = "";
            Game game = (from g in Games
                         where !g.IsStarted() && (g.Name == game_name)
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