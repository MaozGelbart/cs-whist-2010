﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.API
{
    public enum RoundState
    {
        Bidding = 1,
        Contract = 2,
        Playing = 3,
        Results = 4
    }


 
    [Serializable]
    public class RoundStatus
    {
        /// <summary>
        /// Defines the round current state
        /// </summary>
        public RoundState State { get; set; }

        /// <summary>
        /// The index of the player that should play next. 0 is you, 1 is the next clockwise, ..
        /// </summary>
        public int PlayerTurn { get; set; }

        /// <summary>
        /// The player who started the current turn.
        /// </summary>
        public int LeadingPlayer { get; set; }

        /// <summary>
        /// The number of completed turns
        /// </summary>
        public int TurnNumber { get; set; }

        /// <summary>
        /// Array of 4 Biddings, the object at index 0 is the bidding of the you
        /// </summary>
        public Bid?[] Biddings { get; set; }

        /// <summary>
        /// Array of 4 cards, the current cards on the board, starting from you
        /// </summary>
        public Card?[] CurrentPlay { get; set; }

        /// <summary>
        /// Array of 4, Number of tricks for each player, starting from you
        /// </summary>
        public int[] TricksTaken { get; set; }

        /// <summary>
        /// The current round Trump. Null for suit-less
        /// </summary>
        public Suit? Trump { get; set; }

    }

    [Serializable]
    public class GameStatus
    {

        /// <summary>
        /// Names of player, starts with you, continues clockwise
        /// </summary>
        public string[] PlayerNames { get; set; }

        /// <summary>
        /// Scores of players, starts with you, continues clockwise
        /// </summary>
        public int[] Scores { get; set; }

        /// <summary>
        /// Number of rounds played so far
        /// </summary>
        public int RoundNumber { get; set; }

    }
}