using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Helper class, inherit this class to implement a part of player (bidder,card-exchanger, gamer).
    /// Use statuses and cards properties only for reading, is recommended.
    /// </summary>
    /// <typeparam name="IT"></typeparam>
    public abstract class PlayerPartExtender<IT>
    {

        public RoundStatus CurrentRoundStatus
        {
            get;
            set;
        }

        public GameStatus CurrentGameStatus
        {
            get;
            set;
        }

        public List<Card> Cards
        {
            get;
            set;
        }

        public static implicit operator IT(PlayerPartExtender<IT> a)
        {
            return a;
        }
    }
}
