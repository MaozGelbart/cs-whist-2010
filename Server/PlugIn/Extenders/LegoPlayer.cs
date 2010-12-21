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

                //reset memory & empty suits
                m_playedCards = new HashSet<int>[4];
                for (int i = 0; i < 4; i++)
                {
                    m_playedCards[i] = new HashSet<int>();
                }

                m_playerEmptySuits = new HashSet<Suit?>[3];
                for (int i = 0; i < 3; i++)
                {
                    m_playerEmptySuits[i] = new HashSet<Suit?>();
                }

                this.Gamer.UpdateMemory(m_playedCards, m_playerEmptySuits);
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

                //end of play?
                if (value.State == RoundState.TurnResults)
                {
                    calculateRound(value);
                }
            }
        }

        #endregion

        #region LegoPlayer Memory Related

        protected ISet<int>[] m_playedCards;
        protected ISet<Suit?>[] m_playerEmptySuits;

        private void calculateRound(RoundStatus status)
        {
            //check all other players
            for (int i = 0; i < 3; i++)
            {
                //different suit?
                if (status.GetCurrentPlaySuit() != status.GetCurrentPlay((PlayerSeat)i + 1).Value.Suit)
                {
                    m_playerEmptySuits[i].Add(status.GetCurrentPlaySuit());
                }
            }

            //update memory
            updateMemory(status.CurrentPlay);
            this.Gamer.UpdateMemory(m_playedCards, m_playerEmptySuits);
        }

        /// <summary>
        /// happens at the end of each round - insert played cards to memory
        /// </summary>
        /// <param name="roundCards">played cards</param>
        private void updateMemory(Card?[] roundCards)
        {
            foreach (Card c in roundCards)
            {
                m_playedCards[(int)c.Suit - 1].Add(c.Value);
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
