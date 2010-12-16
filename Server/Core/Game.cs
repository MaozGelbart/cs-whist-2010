using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.API;
using System.Threading;

namespace Brain
{
    public class Game
    {
        const int MAXIMUM_TRIES_FOR_AN_UNCOPERATIVE_PLAYER = 4;

        public Game(int num_of_rounds, int milisecs_between_truns)
        {
            this.num_of_rounds = num_of_rounds;
            this.milisecs_between_truns = milisecs_between_truns;

            this.RoundStatus = new RoundStatus[4];
            this.GameStatus = new GameStatus[4];
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i] = new RoundStatus();
                this.GameStatus[i] = new GameStatus();
            }
            this.Players = new Player[4];
        }

        #region Private Members

        private RoundStatus[] RoundStatus { get; set; }

        private GameStatus[] GameStatus { get; set; }

        private Player[] Players { get; set; }

        private int playerMissing = 4;

        private int dealerIndex = 0;

        int playersReponeded = 0;

        int currentAwaitingIndex;

        Bid? currentStrongBid = null;

        int playerPassed;

        int leadingPlayer = 0;

        int lastBidderIndex;

        int totalContract;

        Card? strongestCard;

        int strongestCardPlayer;

        Suit roundSuit;

        Timer tm = null;

        int num_of_rounds;

        int milisecs_between_truns;

        int current_repeating_request = 0;

        int current_responding_player_index = -1;

        #endregion

        #region Public Members

        public IGameViewer Viewer
        {
            get;
            set;
        }

        public bool IsStarted()
        {
            return playerMissing == 0;
        }

        public void AddPlayer(IAsyncPlayer p)
        {
            int playerIndex = 4 - (this.playerMissing--);
            Player newPlayer = new Player(p);
            this.Players[playerIndex] = newPlayer;

            newPlayer.oPlayer.OnUpdateStatusRequested += delegate(object sender, EventArgs e)
            {
                PlayerUpdateStatusRequested(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnGetBidCompleted += delegate (object sender, RecieveBidEventArgs e)
                {
                    PlayerBidAccepted(sender, e, playerIndex);
                };

            newPlayer.oPlayer.OnGetContractCompleted += delegate(object sender, RecieveContractEventArgs e)
            {
                PlayerContractAccepted(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnGetExchangedCardsCompleted += delegate(object sender, RecieveCardsEventArgs e)
            {
                PlayerCardsAccepted(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnGetPlayCompleted += delegate(object sender, RecievePlayEventArgs e)
            {
                PlayerPlayAccepted(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnKillGameRequested += new EventHandler<EventArgs>(oPlayer_OnKillGameRequested);
           
            if (playerMissing == 0)
                StartGame();
        }

        void oPlayer_OnKillGameRequested(object sender, EventArgs e)
        {
            foreach (Player p in this.Players)
            {
                p.oPlayer.RecieveErrorMessage("Someone killed the game");
            }
            GameFactory.KillGame(this);
        }

        #endregion

        #region Player Callbacks

        private void PlayerUpdateStatusRequested(object sender, EventArgs e, int playerIndex)
        {
            this.Players[playerIndex].oPlayer.UpdateRoundStatus(this.RoundStatus[playerIndex]);
        }

        private void PlayerPlayAccepted(object sender, RecievePlayEventArgs e, int playerIndex)
        {
            // check if play is legal
            bool legal = false;
            Player p = this.Players[playerIndex];
            if (p.Cards.Contains(e.Play))
            {
                if (playerIndex != leadingPlayer)
                {
                    if (e.Play.Suit != roundSuit)
                    {
                        if (p.Cards.Count(c => c.Suit == roundSuit) == 0)
                        {
                            legal = true;
                        }
                    }
                    else
                        legal = true;
                }
                else
                    legal = true;
            }
            // if not, ask again
            if (!legal)
            {
                if (current_repeating_request < MAXIMUM_TRIES_FOR_AN_UNCOPERATIVE_PLAYER)
                {
                    current_responding_player_index = playerIndex;
                    current_repeating_request++;
                    p.oPlayer.RequestPlay();
                    return;
                }
                else
                {
                    Exit_EnaughIsEnaugh(current_responding_player_index);
                    return;
                }
            }
            current_repeating_request = 0;
            p.Cards.Remove(e.Play);
            for( int i=0; i<4; i++)
            {
                this.RoundStatus[i].CurrentPlay[(playerIndex - i + 4)%4] = e.Play;
                this.RoundStatus[i].PlayerTurn = (playerIndex + 1 + 4 - i) % 4;
            }
            if (playerIndex == leadingPlayer)
            {
                roundSuit = e.Play.Suit;
                strongestCard = e.Play;
                strongestCardPlayer = playerIndex;
            }
            else
            {
                if (strongestCard.Value.Suit == currentStrongBid.Value.Suit)
                {
                    if (e.Play.Suit == currentStrongBid.Value.Suit && e.Play.Value > strongestCard.Value.Value)
                    {
                        strongestCard = e.Play;
                        strongestCardPlayer = playerIndex;
                    }
                }
                else
                {
                    if (e.Play.Suit == currentStrongBid.Value.Suit || (e.Play.Suit == roundSuit && e.Play.Value > strongestCard.Value.Value))
                    {
                        strongestCard = e.Play;
                        strongestCardPlayer = playerIndex;
                    }
                }
            }
            // if so: update all players && ask the next player to play
            if ((playerIndex + 1) % 4 == leadingPlayer)
            {
                FinishPlayRound();
            }
            else
            {
                UpdateStatusToPlayers(false);
                this.Players[(playerIndex + 1) % 4].oPlayer.RequestPlay();
            }
            // if last player, move to next phase
        }

        private void PlayerCardsAccepted(object sender, RecieveCardsEventArgs e, int playerIndex)
        {
            // check if given cards are legal
            bool legal = false;
            Player p = this.Players[playerIndex];
            if (e.Cards.Length == 3)
            {
                if (p.Cards.Contains(e.Cards[0]) && p.Cards.Contains(e.Cards[1]) && p.Cards.Contains(e.Cards[2]))
                {
                    if (!e.Cards[0].Equals(e.Cards[1]) && !e.Cards[0].Equals(e.Cards[2]) && !e.Cards[1].Equals(e.Cards[2]))
                    {
                        legal = true;
                    }
                }
            }
            // if not ask again
            if (!legal)
            {
                if (current_repeating_request < MAXIMUM_TRIES_FOR_AN_UNCOPERATIVE_PLAYER)
                {
                    current_responding_player_index = playerIndex;
                    current_repeating_request++;
                    p.oPlayer.RequestExhangeCards();
                    return;
                }
                else
                {
                    Exit_EnaughIsEnaugh(current_responding_player_index);
                    return;
                }
            }
            current_repeating_request = 0;
            // if legal check if all players gave cards, 
            playersReponeded |= (1 << playerIndex);
            p.Cards.Remove(e.Cards[0]);
            p.Cards.Remove(e.Cards[1]);
            p.Cards.Remove(e.Cards[2]);
            int targetPlayerIndex = playerIndex + 1 + RoundStatus[0].TurnNumber;
            Player targetPlayer = this.Players[targetPlayerIndex % 4];
            targetPlayer.AwaitingCards = new List<Card>(e.Cards);
            // if so: show all players their new cards, move to next phase
            if (playersReponeded == 15)
            {
                foreach (Player pl in Players)
                {
                    pl.oPlayer.RecieveExchangeCards(pl.AwaitingCards.ToArray());
                    pl.Cards.AddRange(pl.AwaitingCards);
                }
                StartBidding();
            }
        }

        private void PlayerContractAccepted(object sender, RecieveContractEventArgs e, int playerIndex)
        {
            // check if contract is legal
            bool legal = false;
            if( e.Amount > 0 && e.Amount <=13)
            {
                if (playerIndex == leadingPlayer)
                {
                    if (e.Amount >= currentStrongBid.Value.Amount)
                    {
                        legal = true;
                    }
                }
                else
                {
                    if ((playerIndex + 1) % 4 == leadingPlayer)
                    {
                        if (totalContract + e.Amount != 13)
                            legal = true;
                    }
                    else
                        legal = true;
                }
            }
            // if not, ask again
            if (!legal)
            {
                if (current_repeating_request < MAXIMUM_TRIES_FOR_AN_UNCOPERATIVE_PLAYER)
                {
                    current_responding_player_index = playerIndex;
                    current_repeating_request++;
                    this.Players[playerIndex].oPlayer.RequestDeclare();
                    return;
                }
                else
                {
                    Exit_EnaughIsEnaugh(current_responding_player_index);
                    return;
                }
            }
            current_repeating_request = 0;
            totalContract += e.Amount;
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].Biddings[(playerIndex - i + 4) % 4] = new Bid { Suit = null, Amount = e.Amount };
                this.RoundStatus[i].PlayerTurn = (playerIndex + 1 - i + 4) % 4;
            }
            // if so: update all players, ask the next player to declare
            if ((playerIndex + 1) % 4 == leadingPlayer)
            {
                StartPlaying();
            }
            else
            {
                UpdateStatusToPlayers(false);
                this.Players[(playerIndex + 1) % 4].oPlayer.RequestDeclare();
            }
            // if last player, move to next phase
        }

        private void PlayerBidAccepted(object sender, RecieveBidEventArgs e, int playerIndex)
        {
            // check if bid is legal
            bool legal = false;
            if (playerIndex == currentAwaitingIndex)
            {
                if (e.Bid.HasValue)
                {
                    if (e.Bid.Value.Amount >= 5 + RoundStatus[0].TurnNumber && e.Bid.Value.Amount <= 13)
                    {
                        if (currentStrongBid != null)
                        {
                            if (e.Bid.Value.Amount == currentStrongBid.Value.Amount)
                            {
                                if (e.Bid.Value.Suit > currentStrongBid.Value.Suit)
                                    legal = true;
                            }
                            else
                            {
                                if (e.Bid.Value.Amount > currentStrongBid.Value.Amount)
                                    legal = true;
                            }
                        }
                        else
                            legal = true;
                    }
                }
                else
                    legal = true;
            }
            // if not, ask again
            if (!legal)
            {
                if (current_repeating_request < MAXIMUM_TRIES_FOR_AN_UNCOPERATIVE_PLAYER)
                {
                    current_responding_player_index = playerIndex;
                    current_repeating_request++;
                    this.Players[playerIndex].oPlayer.RequestBid();
                    return;
                }
                else
                {
                    Exit_EnaughIsEnaugh(current_responding_player_index);
                    return;
                }
            }
            current_repeating_request = 0;
            // if so: update all players ask the next player to bid
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].Biddings[(playerIndex - i + 4) % 4] = e.Bid;
            }
            if (e.Bid == null)
            {
                playerPassed++;
                this.Players[playerIndex].Passed = true;
            }
            else
            {
                lastBidderIndex = playerIndex;
                currentStrongBid = e.Bid;
            }
            if( playerPassed == 4)
            {
                for( int i=0; i<4; i++)
                    this.RoundStatus[i].TurnNumber++;
                if( this.RoundStatus[0].TurnNumber == 3)
                {
                    StartRound();
                    return;
                }
                else
                {
                    StartFrishing();
                    return;
                }
            }
            do{
            currentAwaitingIndex++;
            currentAwaitingIndex %= 4;
            }
            while( this.Players[currentAwaitingIndex].Passed);
            // if last player, move to next phase
            if (currentAwaitingIndex == lastBidderIndex)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.RoundStatus[i].Trump = currentStrongBid.Value.Suit;
                    this.RoundStatus[i].LeadingPlayer = (lastBidderIndex - i + 4) % 4;
                    this.RoundStatus[i].State = RoundState.Contract;
                }
                leadingPlayer = lastBidderIndex;
                UpdateStatusToPlayers(false);
                StartContract();
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    this.RoundStatus[i].PlayerTurn = (currentAwaitingIndex - i + 4) % 4;
                }
                UpdateStatusToPlayers(false);
                this.Players[currentAwaitingIndex].oPlayer.RequestBid();
            }
        }

        #endregion

        #region Private Logic

        private void StartPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].State = RoundState.Playing;
                this.RoundStatus[i].TurnNumber = 0;
            }
            UpdateStatusToPlayers(false);
            StartPlayRound();
        }

        private void StartPlayRound()
        {
            if( tm != null)
            {
                tm.Dispose();
                tm = null;
            }
            if (this.RoundStatus[0].TurnNumber == 12)
            {
                FinishPlaying();
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].TurnNumber++;
                this.RoundStatus[i].State = RoundState.Playing;

                for (int j = 0; j < 4; j++)
                    this.RoundStatus[i].CurrentPlay[j] = null;
            }
            UpdateStatusToPlayers(false);
            strongestCard = null;
            this.Players[leadingPlayer].oPlayer.RequestPlay();
        }
        private void FinishPlayRound()
        {
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].TricksTaken[(strongestCardPlayer - i + 4) % 4]++;
                this.RoundStatus[i].LeadingPlayer = (strongestCardPlayer - i + 4) % 4;
                
                //update roundstatus state to be "results"
                this.RoundStatus[i].State = RoundState.Results;
            }
            leadingPlayer = strongestCardPlayer;
            UpdateStatusToPlayers(true);
            UpdateStatusToPlayers(false);
            tm = new Timer(new TimerCallback(l => StartPlayRound()), null, Viewer != null ? milisecs_between_truns : 10, Viewer != null ? milisecs_between_truns : 10);
        }

        private void FinishPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                int score = this.RoundStatus[i].Biddings[0].Value.Amount - this.RoundStatus[i].TricksTaken[0];
                if (score == 0)
                {
                    score = (int)Math.Pow(this.RoundStatus[i].TricksTaken[0], 2.0) + 10;
                }
                else
                {
                    score = Math.Abs(score) * -10;
                }
                for (int j = 0; j < 4; j++)
                {
                    this.GameStatus[j].Scores[(i - j + 4) % 4] += score;
                }
            }
            CompleteRound();
        }

        private void StartContract()
        {
            UpdateStatusToPlayers(true);
            totalContract = 0;
            Player p = this.Players[leadingPlayer];
            p.oPlayer.RequestDeclare();
        }

        private void StartGame()
        {
            // update player names and scores
            var playerNames = (from p in Players
                               select p.oPlayer.Name).ToArray();
            for (int i = 0; i < 4; i++)
            {
                this.GameStatus[i].PlayerNames = GetArrayFrom(playerNames, i);
                this.GameStatus[i].Scores = new int[4] { 0, 0, 0, 0 };


                this.GameStatus[i].RoundNumber = 0;
            }
            UpdateGameStatusToPlayers();
            dealerIndex = 0;
            StartRound();
        }

        private T[] GetArrayFrom<T>(T[] array, int startIndex)
        {
            T[] new_arr = new T[4];
            for (int i = 0; i < 4; i++)
            {
                new_arr[i] = array[(startIndex + i) % 4];
            }
            return new_arr;
        }

        private void CompleteRound()
        {
            /// TODO : calculate scores acording to bids and tricks(takes)
            ///     update scores to game status
            ///     

            // move dealer to next player
            this.dealerIndex++;
            this.dealerIndex %= 4;


            for (int i = 0; i < 4; i++)
            {
                this.GameStatus[i].RoundNumber++;
            }
            UpdateGameStatusToPlayers();
            if (this.GameStatus[0].RoundNumber == num_of_rounds - 1)
                GameFactory.KillGame(this);
            else
                StartRound();
        }

        private void StartRound()
        {

            // update round status
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].LeadingPlayer = (this.dealerIndex - i + 4) % 4;
                this.RoundStatus[i].PlayerTurn = (this.dealerIndex - i + 4) % 4;
                this.RoundStatus[i].State = RoundState.Bidding;
                this.RoundStatus[i].Biddings = new Bid?[4];
                this.RoundStatus[i].Trump = null;
                this.RoundStatus[i].TurnNumber = 0;
                this.RoundStatus[i].TricksTaken = new int[4] { 0, 0, 0, 0 };
                this.RoundStatus[i].CurrentPlay = new Card?[4];
                this.Players[i].Cards = new List<Card>();
            }

            UpdateStatusToPlayers(true);
            UpdateStatusToPlayers(false);
            // give cards
            List<Card> cards = new List<Card>(52);
            for (int n = 2; n <= 14; n++)
            {
                for (int s = 1; s <= 4; s++)
                {
                    cards.Add(new Card((Suit)s, n));
                }
            }
            Random r = new Random();
            for (int i = 0; i < 13; i++)
            {
                foreach (Player p in Players)
                {
                    Thread.Sleep(30);
                    int cardNum = r.Next(cards.Count);
                    Card c = cards[cardNum];
                    cards.Remove(c);
                    p.Cards.Add(c);
                }
            }
            foreach (Player p in Players)
            {
                p.GiveCards();
            }


            StartFrishing();
        }



        private void StartFrishing()
        {
            playersReponeded = 0;
            foreach (Player p in Players)
            {
                p.oPlayer.RequestExhangeCards();
            }

        }

        private void StartBidding()
        {
            for (int i = 0; i < 4; i++)
            {
                this.Players[i].Passed = false;
                this.RoundStatus[i].LeadingPlayer = (this.dealerIndex - i + 4) % 4;
            }
            UpdateStatusToPlayers(true);
            lastBidderIndex = -1;
            playerPassed = 0;
            currentStrongBid = null;
            Player first = Players[this.dealerIndex];
            currentAwaitingIndex = this.dealerIndex;
            first.oPlayer.RequestBid();
        }

        private void UpdateStatusToPlayers(bool updateViewer)
        {
            Card[][] allCards = new Card[4][];
            for (int i = 0; i < 4; i++)
            {
                if( Viewer != null && updateViewer)
                    allCards[i] = this.Players[i].Cards.ToArray();
                else
                    this.Players[i].oPlayer.UpdateRoundStatus(this.RoundStatus[i]);
            }
            if (Viewer != null && updateViewer)
            {
                Viewer.UpdateRoundStatus(this.RoundStatus[0], allCards);
                Thread.Sleep(milisecs_between_truns);
            }
        }

        private void UpdateGameStatusToPlayers()
        {
            for (int i = 0; i < 4; i++)
            {
                this.Players[i].oPlayer.UpdateGameStatus(this.GameStatus[i]);
            }
            if (Viewer != null)
                Viewer.UpdateGameStatus(this.GameStatus[0]);
        }

        private void Exit_EnaughIsEnaugh(int current_responding_player_index)
        {
            string msg = "Player no. " + current_responding_player_index + " is repeatedly making illegal actions. must end this";
            if (Viewer != null)
            {
                Viewer.RecieveErrorMessage(msg);
                GameFactory.KillGame(this);
            }
            else
            {
                foreach( Player p in this.Players)
                {
                    p.oPlayer.RecieveErrorMessage(msg);
                }
                GameFactory.KillGame(this);
            }
        }

        #endregion
    }
}