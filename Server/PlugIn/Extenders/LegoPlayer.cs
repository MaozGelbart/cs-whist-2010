using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.API
{
    /// <summary>
    /// Base class uses to make a combination player from small parts. must implement a no arguments constructor that uses 
    /// :base(bidder,cardExchanger, gamer) for constructing
    /// </summary>
    public abstract class LegoPlayer : PlayerBase, IPlayer
    {
        private BidderBase Bidder;
        private CardExchangerBase CardExchanger;
        private GamerBase Gamer;

        public LegoPlayer(BidderBase bidder, CardExchangerBase cardExchanger, GamerBase gamer)
        {
            this.Bidder = bidder;
            this.CardExchanger = cardExchanger;
            this.Gamer = gamer;
        }

        #region Protected Methods

        protected void ReplaceBidder(BidderBase bidder)
        {
            Bidder = bidder;
            ReplacePart(bidder);
        }

        protected void ReplaceCardExchanger(CardExchangerBase cardExchanger)
        {
            this.CardExchanger = cardExchanger;
            ReplacePart(cardExchanger);
        }

        protected void ReplaceGamer(GamerBase gamer)
        {
            this.Gamer = gamer;
            this.ReplacePart(gamer);
        }

        private void ReplacePart(PlayerPartExtender part)
        {
            part.Cards = this.Cards;
            part.CurrentGameStatus = this.CurrentGameStatus;
            part.CurrentRoundStatus = this.CurrentRoundStatus;
        }

        #endregion

        #region Overrides

        protected override List<Card> Cards
        {
            get
            {
                return base.Cards;
            }
            set
            {
                base.Cards = value;
                this.Bidder.Cards = value;
                this.CardExchanger.Cards = value;
                this.Gamer.Cards = value;
            }
        }

        protected override GameStatus CurrentGameStatus
        {
            get
            {
                return base.CurrentGameStatus;
            }
            set
            {
                base.CurrentGameStatus = value;
                this.Bidder.CurrentGameStatus = value;
                this.CardExchanger.CurrentGameStatus = value;
                this.Gamer.CurrentGameStatus = value;
            }
        }

        protected override RoundStatus CurrentRoundStatus
        {
            get
            {
                return base.CurrentRoundStatus;
            }
            set
            {
                base.CurrentRoundStatus = value;
                this.Bidder.CurrentRoundStatus = value;
                this.CardExchanger.CurrentRoundStatus = value;
                this.Gamer.CurrentRoundStatus = value;
            }
        }

        #endregion

        #region ICardExchanger Members

        public virtual Card[] RequestExhangeCards()
        {
            var cards = CardExchanger.RequestExhangeCards();
            this.PassCards(cards[0], cards[1], cards[2]);
            return cards;
        }

        #endregion

        #region IBidder Members

        public virtual Bid? RequestBid()
        {
            return Bidder.RequestBid();
        }

        public virtual int RequestDeclare()
        {
            return Bidder.RequestDeclare();
        }

        #endregion

        #region IPlayerGamer Members

        public virtual Card RequestPlay()
        {
            Card c = Gamer.RequestPlay();
            ThrowCard(c);
            return c;
        }

        #endregion
    }
}
