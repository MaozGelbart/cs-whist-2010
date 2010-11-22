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
        private PlayerPartExtender<IBidder> Bidder;
        private PlayerPartExtender<ICardExchanger> CardExchanger;
        private PlayerPartExtender<IPlayerGamer> Gamer;

        public LegoPlayer(PlayerPartExtender<IBidder> bidder, PlayerPartExtender<ICardExchanger> cardExchanger, PlayerPartExtender<IPlayerGamer> gamer)
        {
            this.Bidder = bidder;
            this.CardExchanger = cardExchanger;
            ((ICardExchanger)this.CardExchanger).OnGetExchangedCardsCompleted += OnGetExchangeCards;
            this.Gamer = gamer;
            ((IPlayerGamer)this.Gamer).OnGetPlayCompleted += OnGetPlay;
        }

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

        public void RequestExhangeCards()
        {
            ((ICardExchanger)CardExchanger).RequestExhangeCards();
        }

        public event EventHandler<RecieveCardsEventArgs> OnGetExchangedCardsCompleted;

        private void OnGetExchangeCards(object sernder, RecieveCardsEventArgs e)
        {
            this.PassCards(e.Cards[0], e.Cards[1], e.Cards[2]);

            if (OnGetExchangedCardsCompleted != null)
                OnGetExchangedCardsCompleted(this, e);
        }

        #endregion

        #region IBidder Members

        public void RequestBid()
        {
            ((IBidder)Bidder).RequestBid();
        }

        public void RequestDeclare()
        {
            ((IBidder)Bidder).RequestDeclare();
        }

        public event EventHandler<RecieveBidEventArgs> OnGetBidCompleted
        {
            add { ((IBidder)Bidder).OnGetBidCompleted += value; }
            remove { ((IBidder)Bidder).OnGetBidCompleted -= value; }
        }

        public event EventHandler<RecieveContractEventArgs> OnGetContractCompleted
        {
            add { ((IBidder)Bidder).OnGetContractCompleted += value; }
            remove { ((IBidder)Bidder).OnGetContractCompleted -= value; }
        }

        #endregion

        #region IPlayerGamer Members

        public void RequestPlay()
        {
            ((IPlayerGamer)Gamer).RequestPlay();
        }

        void OnGetPlay(object sernder, RecievePlayEventArgs e)
        {
            ThrowCard(e.Play);
            if (OnGetPlayCompleted != null)
                OnGetPlayCompleted(this, e);
        }

        public event EventHandler<RecievePlayEventArgs> OnGetPlayCompleted;

        #endregion
    }
}
