using System;
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
        TurnResults = 4
    }


    public enum PlayerSeat
    {
        Self = 0,
        West = 1,
        North = 2,
        East = 3
    }


 
    /// <summary>
    /// This object sent to players on every change in the game, during the game rounds
    /// </summary>
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
        public PlayerSeat PlayerTurn { get; set; }

        /// <summary>
        /// The player who started the current turn.
        /// </summary>
        public PlayerSeat LeadingPlayer { get; set; }

        /// <summary>
        /// The number of completed turns
        /// </summary>
        public int TurnNumber { get; set; }

        /// <summary>
        /// Array of 4 Biddings, the object at index 0 is the bidding of the you
        /// </summary>
        public Bid?[] Biddings { get; set; }

        public Bid? GetBid(PlayerSeat seat) { return Biddings[(int)seat]; }

        /// <summary>
        /// Returns boolean indicating if bidding is over or under 13
        /// </summary>
        public bool IsBiddingUnder 
        {
            get
            {
                int sum = 0;
                foreach (Bid b in Biddings)
                {
                    sum += b.Amount;
                }

                return (sum < 13);
            }

        }


        /// <summary>
        /// Array of 4 cards, the current cards on the board, starting from you
        /// </summary>
        public Card?[] CurrentPlay { get; set; }

        public Card? GetCurrentPlay(PlayerSeat seat) 
        {
            return CurrentPlay[(int)seat]; 
        }

        public Suit? GetCurrentPlaySuit() 
        {
            if (GetCurrentPlay((PlayerSeat)LeadingPlayer).HasValue)
            {
                return CurrentPlay[(int)LeadingPlayer].Value.Suit;
            }
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// Array of 4, Number of tricks for each player, starting from you
        /// </summary>
        public int[] TricksTaken { get; set; }

        public int GetTricks(PlayerSeat seat) { return TricksTaken[(int)seat]; }

        /// <summary>
        /// The current round Trump. Null for suit-less
        /// </summary>
        public Suit? Trump { get; set; }

        public RoundStatus Clone()
        {
            return new RoundStatus
            {
                State = this.State,
                PlayerTurn = this.PlayerTurn,
                LeadingPlayer = this.LeadingPlayer,
                Trump = this.Trump,
                TricksTaken = (int[])this.TricksTaken.Clone(),
                Biddings = (Bid?[])this.Biddings.Clone(),
                TurnNumber = this.TurnNumber,
                CurrentPlay = (Card?[])this.CurrentPlay.Clone()
            };
        }

    }

    /// <summary>
    /// This object is sent to the players only on round begining or end
    /// </summary>
    [Serializable]
    public class GameStatus
    {

        /// <summary>
        /// Names of player, starts with you, continues clockwise
        /// </summary>
        public string[] PlayerNames { get; set; }

        /// <summary>
        /// strings representing the types of the player ( the fullname of their class)
        /// </summary>
        public string[] PlayerTypes { get; set; }

        /// <summary>
        /// Scores of players, starts with you, continues clockwise
        /// </summary>
        public int[] Scores { get; set; }

        /// <summary>
        /// Number of rounds played so far
        /// </summary>
        public int RoundNumber { get; set; }

        public GameStatus Clone()
        {
            return new GameStatus { PlayerNames = (string[])this.PlayerNames.Clone(), RoundNumber = this.RoundNumber, Scores = (int[])this.Scores.Clone() };
        }

    }
}