using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.API;

namespace PlugIn.Gamers
{
    /// <summary>
    /// This gamer tries to minimize |Bidding[self] - RoundsTaken[self]|
    /// </summary>
    class SmartGamer : GamerBase, IGamer
    {
        #region private fields

        /// <summary>
        /// Holds self declaration
        /// </summary>
        private Bid _selfDeclaration;

        /// <summary>
        /// True if the game is an under game
        /// </summary>
        private bool _underGame;

        #endregion

        #region GamerBase Members

        public override Card RequestPlay()
        {
            //initialize private fields
            if (CurrentRoundStatus.TurnNumber == 1)
            {
                InitializeGame();
            }

            //updates private fields from last round
            UpdateStatus();

            //if mission achieved (or player took more tricks than self bid) - avoid taking more tricks
            if (_selfDeclaration.Amount - CurrentRoundStatus.GetTricks(PlayerSeat.Self) <= 0)
            {
                return AvoidAllTricks();
            }

            //case of under game 
            if (_underGame)
            {
                return UnderGameMove();
            }

            else //case of over game
            {
                return OverGameMove();
            }
        }


        #endregion

        /// <summary>
        /// Initializes gameplay private members
        /// </summary>
        private void InitializeGame()
        {
            _selfDeclaration = CurrentRoundStatus.GetBid(PlayerSeat.Self).Value;

            int totalTricksBid = 0;
            for (int i = 0; i < 4; i++)
            {
                totalTricksBid += CurrentRoundStatus.GetBid((PlayerSeat)i).Value.Amount;             
            }
            
            if (totalTricksBid < 13)
            {
                _underGame = true;
            }
            
            else
            {
                _underGame = false;
            }
        }

        /// <summary>
        /// Updates the gameplay private members
        /// </summary>
        private void UpdateStatus()
        {
            
        }

        /// <summary>
        /// Tries to avoid all tricks, does not consider future tricks
        /// </summary>
        /// <returns></returns>
        private Card AvoidAllTricks()
        {
            //if someone started, throw highest card available
            if (this.CurrentRoundStatus.LeadingPlayer != PlayerSeat.Self)
            {
                Card chosen = default(Card);
                try
                {
                    chosen = ThrowHighestAvoidCardAvailable();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.StackTrace);
                }
                return chosen;
            }

            //lose control, throw lowest card available that enables to lose control
            else
            {
                Card chosen = default(Card);
                try
                {
                    chosen = LoseControl();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.StackTrace);
                }
                return chosen;
            }
        }

        /// <summary>
        /// When Self starts, try to lose control 
        /// </summary>
        /// <returns></returns>
        private Card LoseControl()
        {
            Card chosen = default(Card);
            List<Card> nominees = new List<Card>();
            double lowestChance = 200;

            foreach (Card c in Cards)
            {
                double chance = ChanceToLoseTheHand(c);
                if (chance < lowestChance)
                {
                    lowestChance = chance;
                    nominees.Clear();
                }
                if (chance == lowestChance)
                {
                    nominees.Add(c);
                }
            }

            //TODO can make it take the shortest suit
            chosen = (from c in nominees
                      orderby c.Value ascending
                      select c).FirstOrDefault();

            //sanity check, code should never get here
            if (chosen.IsEmpty())
            {
                chosen = Cards.FirstOrDefault();
            }

            return chosen;
        }

        /// <summary>
        /// When someone else started, throws the highest card available for not taking the round
        /// </summary>
        /// <returns></returns>
        private Card ThrowHighestAvoidCardAvailable()
        {
            Suit turnSuit = CurrentRoundStatus.GetCurrentPlaySuit().Value;
            Card chosen = default(Card);

            //sort current highest cards
            List<Card>[] cardArr = ArrangeCardBySuits(Cards);

            //get current play cards into list
            List<Card> leadingSuitCardsPlayed = new List<Card>();

            //case we have from leading suit
            if (GetCardsBySuit(cardArr, turnSuit).Count != 0)
            {
                //find highest card from leading suit
                foreach (Card? card in CurrentRoundStatus.CurrentPlay)
                {
                    if (card.HasValue && card.Value.Suit == turnSuit)
                    {
                        leadingSuitCardsPlayed.Add(card.Value);
                    }
                }
                Card highestCardOnBoard = default(Card);

                //find this round highest card
                highestCardOnBoard = GetHighestCardInCollection(leadingSuitCardsPlayed);

                //find the highest card that is lower than the highest leading suit card on board, and is same suit
                chosen = GetHighestLowerCard(GetCardsBySuit(ArrangeCardBySuits(Cards), turnSuit), highestCardOnBoard);

                //if we have cards from leading suit, but not lower than card played, we'll throw our lowest card from that suit
                if (chosen.IsEmpty())
                {
                    chosen = GetLowestCardInCollection(GetCardsBySuit(ArrangeCardBySuits(Cards), turnSuit));
                }
            }

            else //case we don't have cards from leading suit
            {
                List<Card> nominees = new List<Card>();
                //get cards from all suits who are not trump suit
                foreach (List<Card> suitCards in cardArr)
                {
                    if (suitCards.Count != 0 && suitCards.First().Suit != CurrentRoundStatus.Trump)
                    {
                        nominees.AddRange(suitCards);
                    }
                }

                //check for having cards other than trumps
                if (nominees.Count != 0)
                {
                    //return highest card that is not a trump
                    //TODO can make it smarter, like throwing a card from the shortest suit
                    chosen = GetHighestCardInCollection(nominees);
                }

                else //case we have only trumps left, check if someone else already cut
                {
                    //find highest trump thrown (if any)
                    Card highestTrump = default(Card);
                    foreach (Card? c in CurrentRoundStatus.CurrentPlay)
                    {
                        if (c.HasValue && c.Value.Suit == CurrentRoundStatus.Trump)
                        {
                            if (highestTrump.IsEmpty() || c.Value.Value > highestTrump.Value)
                            {
                                highestTrump = c.Value;
                            }
                        }
                    }

                    //case that trumps were thrown, find the highest trump that is lower than the highest trump on board
                    chosen = GetHighestLowerCard(GetCardsBySuit(ArrangeCardBySuits(Cards), CurrentRoundStatus.Trump.Value), highestTrump);

                    //if we don't have a lower trump, throw lowest trump
                    //TODO can make it smarter, using statistics
                    if (chosen.IsEmpty())
                    {
                        chosen = GetLowestCardInCollection(GetCardsBySuit(ArrangeCardBySuits(Cards), CurrentRoundStatus.Trump.Value));
                    }
                }
            }

            if (chosen.IsEmpty()) //sanity check, should never evaluate to true
            {
                chosen = Cards.First();
            }
            return chosen;
        }

        /// <summary>
        /// A move that avoids taking one trick
        /// </summary>
        /// <returns></returns>
        private Card AvoidOneTrick()
        {
            Card chosen = default(Card);

            //case someone else started, avoid the take
            if (CurrentRoundStatus.LeadingPlayer != PlayerSeat.Self)
            {
                chosen = ThrowHighestAvoidCardAvailable();
            }

            else //we start, avoid taking the round
            {
                chosen = LoseControl();
            }

            return chosen;
        }

        /// <summary>
        /// returns number of rounds left (positive value)
        /// </summary>
        /// <returns>number of rounds left</returns>
        private int RoundsLeft()
        {
            return 13 - CurrentRoundStatus.TurnNumber;
        }

        /// <summary>
        /// A move for under game
        /// </summary>
        /// <returns>under game move</returns>
        private Card UnderGameMove()
        {
            //case of more rounds left than tricks - avoid taking more tricks
            if (RoundsLeft() - CurrentRoundStatus.GetTricks(PlayerSeat.Self) > 0)
            {
                Card chosen = default(Card);
                try
                {
                    chosen = AvoidOneTrick();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.StackTrace);
                }
                return chosen;
            }

            //case of less rounds left than tricks - must take everything
            else
            {
                Card chosen = default(Card);
                try
                {
                    chosen = TakeOrThrowLowest();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.StackTrace);
                }
                return chosen;
            }
        }

        /// <summary>
        /// A move for over game
        /// </summary>
        /// <returns>an over game move</returns>
        private Card OverGameMove()
        {
            Card chosen = default(Card);
            try
            {
                chosen = TakeOrThrowLowest();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.StackTrace);
            }
            return chosen;
            
        }

        /// <summary>
        /// This method tries to take a round or lose the lowest card possible
        /// </summary>
        /// <returns>A card to play</returns>
        private Card TakeOrThrowLowest()
        {
            Card chosen = default(Card);
            List<Card> nominees = new List<Card>();
            //case someone else started
            if (CurrentRoundStatus.LeadingPlayer != PlayerSeat.Self)
            {
                //get nominees from current suit
                nominees = GetCardsBySuit(ArrangeCardBySuits(Cards), CurrentRoundStatus.GetCurrentPlaySuit().Value);

                //if no card from current suit, set current suit to trumps (if trump game)
                if (nominees.Count == 0 && CurrentRoundStatus.Trump.HasValue && CurrentRoundStatus.Trump.Value != CurrentRoundStatus.GetCurrentPlaySuit().Value)
                {
                    nominees = GetCardsBySuit(ArrangeCardBySuits(Cards), CurrentRoundStatus.Trump.Value);
                }

                //if no trumps in hand, throw the weakest card
                if (nominees.Count == 0)
                {
                    return GetLowestCardInCollection(Cards);
                }


                chosen = (from c in nominees
                          orderby ChanceToLoseTheHand(c) ascending
                          orderby c.Value ascending
                          select c).FirstOrDefault();

                if (ChanceToLoseTheHand(chosen) == 1)
                {
                    chosen = (from c in nominees
                              orderby c.Value ascending
                              select c).FirstOrDefault();
                }

                //sanity check
                if (chosen.IsEmpty())
                {
                    chosen = nominees.FirstOrDefault();
                }
            }

            //case we start
            else
            {
                chosen = (from c in Cards
                          orderby ChanceToLoseTheHand(c) ascending
                          orderby c.Value ascending
                          select c).FirstOrDefault();
                
                if (ChanceToLoseTheHand(chosen) == 1)
                {
                    chosen = (from c in Cards
                              orderby c.Value ascending
                              select c).FirstOrDefault();
                }
            }

            return chosen;
        }

        /// <summary>
        /// returns the suit index inside the sorted suits array
        /// </summary>
        /// <param name="suit">the suit needed</param>
        /// <returns>an int represents the index of that suit inside sorted suits array</returns>
        private static int SuitIndex(Suit suit)
        {
            return (int)suit - 1;
        }

        /// <summary>
        /// returns a list that holds all the cards from a supplied suit
        /// </summary>
        /// <param name="cardsArr">Cards list array representing all suits</param>
        /// <param name="suit">The wanted suit</param>
        /// <returns>A list that holds all suit cards</returns>
        private static List<Card> GetCardsBySuit(List<Card>[] cardsArr, Suit suit)
        {
            return cardsArr[SuitIndex(suit)];
        }

        /// <summary>
        /// returns the highest value card that is lower than the card supplied value 
        /// </summary>
        /// <param name="cardsList">a list of cards (should all be same suit)</param>
        /// <param name="refCard">the card to compare to</param>
        /// <returns>the highest lower card</returns>
        private static Card GetHighestLowerCard(List<Card> cardsList, Card refCard)
        {
            Card chosen = default(Card);

            chosen = (from c in cardsList
                      where c.Value < refCard.Value
                      orderby c.Value descending
                      select c).FirstOrDefault();

            return chosen;
        }

        /// <summary>
        /// calculate the cumulative chance that we lose the hand with this card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private double ChanceToLoseTheHand(Card card)
        {
            double ret = 0;           
            
            //case that on board there's a higher card with same suit, it's a sure lose, return 1
            foreach (Card? c in CurrentRoundStatus.CurrentPlay)
            {
                if (c.HasValue && c.Value.Suit == card.Suit && c.Value.Value > card.Value)
                {
                    return 1;
                }
            }

            //get the players who didn't play yet
            List<PlayerSeat> toPlay = new List<PlayerSeat>();
            for (int i = 1; i < 4; i++)
            {
                if (!CurrentRoundStatus.CurrentPlay[i].HasValue)
                {
                    toPlay.Add((PlayerSeat)i);
                }
            }

            //if someone cut the round, and our card is not a trump, it's a sure lose, return 1
            //if someone who didn't play yet has only trumps left, it's a sure lose, return 1
            if (CurrentRoundStatus.Trump.HasValue)
            {
                foreach (Card? c in CurrentRoundStatus.CurrentPlay)
                {
                    if (c.HasValue && c.Value.Suit == CurrentRoundStatus.Trump)
                    {
                        if (card.Suit != CurrentRoundStatus.Trump)
                        {
                            return 1;
                        }
                    }
                }

                ////check if some player has only trumps left, and he didn't play yet
                //for (PlayerSeat p = PlayerSeat.West ; p <= PlayerSeat.East; p++)
                //{
                //    if (!CurrentRoundStatus.CurrentPlay[(int)p].HasValue)
                //    {
                //        bool hasOtherSuit = false;

                //        for (Suit suit = Suit.Clubs; suit <= Suit.Spades; suit++)
                //        {
                //            if (suit != CurrentRoundStatus.Trump.Value && getPlayerSuitStatus(p, suit))
                //            {
                //                hasOtherSuit = true;
                //            }
                //        }

                //        if (!hasOtherSuit)
                //        {
                //            return 1;
                //        }
                //    }
                //}
            }

            //so far we know that: noone trumped with a higher card, no higher card on board, and noone will surely trump us
            //means we must check statistics

            //create all cards who weren't thrown yet from our cards suit
            List<Card> nominees = new List<Card>();
            for (int i = 2; i <= 14; i++)
            {
                Card c = new Card(card.Suit, i);
                if (!m_playedCards[SuitIndex(c.Suit)].Contains(c.Value) && Cards.Contains(c))
                {
                    nominees.Add(c);
                }
            }
          
            //if someone else started the round
            if (CurrentRoundStatus.LeadingPlayer != PlayerSeat.Self)
            {              
                int cardsFromSuitOnBoard = 0;
                
                foreach (Card? c in CurrentRoundStatus.CurrentPlay)
                {
                    if (c.HasValue && c.Value.Suit == card.Suit)
                    {
                        cardsFromSuitOnBoard++;
                    }
                }

                //if all cards left from this suit are in our hand
                if (ArrangeCardBySuits(Cards)[SuitIndex(card.Suit)].Count() + m_playedCards[SuitIndex(card.Suit)].Count() == 13)
                {
                    //return 1 if we trump, otherwise 0 (no higher card on board was checked before)
                    //TODO if we trump it, maybe others trump as well. check this out in new version
                    if (CurrentRoundStatus.Trump.HasValue && card.Suit == CurrentRoundStatus.Trump.Value)
                    {
                        return 0;
                    }
                    else //means we won't trump this round
                    {
                        return 1;
                    }
                }

                //if we're the last to play, and noone has a higher card than our card
                if (toPlay.Count == 0)
                {
                    //case we're playing the leading suit, means we have highest card therefore chance to lose is 0
                    if (card.Suit == CurrentRoundStatus.GetCurrentPlaySuit().Value)
                    {
                        return 0;
                    }
                    //case we're playing another suit
                    else
                    {
                        return 1;
                    }
                }

                //if some player has a card from wanted suit, and maybe some other players played 
                else
                {

                    double totalProb = 0;

                    foreach (PlayerSeat p in toPlay)
                    {
                        foreach (Card c in nominees)
                        {
                            if (c.Value > card.Value)
                            {
                                totalProb += getCardStatistic(p,c);
                            }
                        }
                    }

                    totalProb /= toPlay.Count;
                    ret = totalProb;
                }

            }

            //if we started the round
            //means no cards from suit on board, and noone has only trumps left or this suit over
            else
            {
                //for a game with trumps, when leading a non-trump
                if (CurrentRoundStatus.Trump.HasValue && card.Suit != CurrentRoundStatus.Trump.Value)
                {
                    int relevant = 0, cutters = 0;
                    double totalProb = 0;
                    

                    for (PlayerSeat p = PlayerSeat.West; p <= PlayerSeat.East; p++)
                    {
                        //if a player has our suit
                        if (getPlayerSuitStatus(p, card.Suit))
                        {
                            relevant++;
                            foreach (Card c in nominees)
                            {
                                if (c.Value > card.Value)
                                {
                                    totalProb += getCardStatistic(p, c);
                                }
                            }
                        }

                        //if a player doesn't have our suit
                        else
                        {
                            //if the player is able to trump us
                            if (getPlayerSuitStatus(p, CurrentRoundStatus.Trump.Value))
                            {
                                //add chance to random trump-throw
                                cutters++;

                                //TODO logic may assume smart player or no smart player (total rounds taken)
                                //here we will assume smartness

                            }
                            
                            //if the player isn't able to trump us, don't calculate him in the percentage count
                            else
                            {

                            }
                        }
                    }
                    //if noone will trump us, and noone has cards from our suit
                    if (relevant == 0 && cutters == 0)
                    {
                        return 0;
                    }

                    if (relevant != 0)
                    {
                        //average total probability over relevant players
                        totalProb /= relevant;
                    }

                    if (cutters != 0)
                    {
                        //add chance that we'll be cut over: number of trumps left  - round left 
                        totalProb += (13 - m_playedCards[SuitIndex(CurrentRoundStatus.Trump.Value)].Count) / (cutters * (13 - CurrentRoundStatus.TurnNumber + 1));
                    }

                }

                //for a game with no-trump, or trying to lead a trump
                else
                {
                    double totalProb = 0;
                    int relevant = 0;
                    for (PlayerSeat p = PlayerSeat.West; p <= PlayerSeat.East; p++)
                    {
                        if (getPlayerSuitStatus(p, card.Suit))
                        {
                            relevant++;
                           
                            foreach (Card c in nominees) {
                                totalProb += getCardStatistic(p, c);
                            }
                        }
                    }
                    totalProb /= relevant;
                    ret = totalProb;
                }
            }

            return ret;
        }

    }
}
