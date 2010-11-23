using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;

namespace Brain
{
    public class Player
    {
        public IAsyncPlayer oPlayer;

        private Player()
        {
            Cards = new List<Card>();
        }

        public Player(IAsyncPlayer obj)
            :base()
        {
            this.oPlayer = obj; 
        }

        public List<Card> Cards { get; set; }

        public List<Card> AwaitingCards { get; set; }

        public Bid CurrentBid { get; set; }

        public bool Passed { get; set; }

        internal void GiveCards()
        {
            oPlayer.RecieveCards(Cards.ToArray());
        }
    }
}