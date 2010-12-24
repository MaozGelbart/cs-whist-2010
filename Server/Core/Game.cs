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
        /// <summary>
        /// number of tries each player has for performing the same simple request before the game is killed
        /// </summary>
        const int MAXIMUM_TRIES_FOR_AN_UNCOPERATIVE_PLAYER = 4;

        /// <summary>
        /// The minimum time span, without any of the player responding, before the game is killed
        /// </summary>
        public static TimeSpan TIME_TO_CALL_GAME_DEATH = TimeSpan.FromSeconds(120);

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num_of_rounds">number of rounds in game</param>
        /// <param name="milisecs_between_truns">time to wait between turns</param>
        /// <param name="name">name of the game</param>
        public Game(int num_of_rounds, int milisecs_between_truns, string name)
        {
            this.Name = name;
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
            ResponseRecieved();
        }

        #endregion

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

        Timer gameKiller;

        DateTime gamePlannedDeathTime;

        #endregion

        #region Public Members

        /// <summary>
        /// Name of the game
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The viewer object recieves all the information about what happened in the game
        /// </summary>
        public IGameViewer Viewer
        {
            get;
            set;
        }

        /// <summary>
        /// Is this game started or we're still waiting for players to register
        /// </summary>
        /// <returns></returns>
        public bool IsStarted()
        {
            return playerMissing == 0;
        }

        public void AddPlayer(IAsyncPlayer p)
        {
            AddPlayer(p, "_human");
        }

        /// <summary>
        /// Adds player to the game
        /// </summary>
        /// <param name="p">Implementation of a player</param>
        public void AddPlayer(IAsyncPlayer p, string type)
        {
            // assign index, create player object
            int playerIndex = 4 - (this.playerMissing--);
            Player newPlayer = new Player(p);
            newPlayer.Type = type;
            this.Players[playerIndex] = newPlayer;
           
            // register all event of the player
            newPlayer.oPlayer.OnUpdateStatusRequested += delegate(object sender, EventArgs e)
            {
                ResponseRecieved();
                PlayerUpdateStatusRequested(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnGetBidCompleted += delegate (object sender, RecieveBidEventArgs e)
                {
                    ResponseRecieved();
                    PlayerBidAccepted(sender, e, playerIndex);
                };

            newPlayer.oPlayer.OnGetContractCompleted += delegate(object sender, RecieveContractEventArgs e)
            {
                ResponseRecieved();
                PlayerContractAccepted(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnGetExchangedCardsCompleted += delegate(object sender, RecieveCardsEventArgs e)
            {
                ResponseRecieved();
                PlayerCardsAccepted(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnGetPlayCompleted += delegate(object sender, RecievePlayEventArgs e)
            {
                ResponseRecieved();
                PlayerPlayAccepted(sender, e, playerIndex);
            };

            newPlayer.oPlayer.OnKillGameRequested += new EventHandler<EventArgs>(oPlayer_OnKillGameRequested);

            newPlayer.oPlayer.OnSendChatMessage += delegate(object sender, RecieveChatMessageEventArgs e)
            {
                ResponseRecieved();
                PlayerSentMessage(sender, e, playerIndex);
            };

            ResponseRecieved();
           
            // if we have enough players, start playing
            if (playerMissing == 0)
                StartGame();
        }

        #endregion

        #region Player Callbacks

        /// <summary>
        /// Gives the game more time to live.
        /// Every game has a planned time of death unless one of players does an action.
        /// To prevent from dead games to be kept in memory of the server
        /// </summary>
        private void ResponseRecieved()
        {
            gamePlannedDeathTime = DateTime.Now.Add(TIME_TO_CALL_GAME_DEATH);
            if (gameKiller == null)
            {
                gameKiller = new Timer(
                    delegate(object obj)
                    {
                        // check if the time for the game to die has passed
                        if (gamePlannedDeathTime < DateTime.Now)
                        {
                            GameFactory.KillGame(this);
                            gameKiller.Dispose();
                        }
                    }
                    , null, TIME_TO_CALL_GAME_DEATH, TIME_TO_CALL_GAME_DEATH);
            }
        }

        /// <summary>
        /// When player (playerIndex) sends a message (e.Message)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="playerIndex"></param>
        private void PlayerSentMessage(object sender, RecieveChatMessageEventArgs e, int playerIndex)
        {
            int i=0;
            foreach (Player p in this.Players)
            {
                p.oPlayer.RecieveChatMessage((PlayerSeat)((playerIndex - i + 4) % 4), e.Message);
                i++;
            }
        }

        /// <summary>
        /// When someone asked to kill the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void oPlayer_OnKillGameRequested(object sender, EventArgs e)
        {
            foreach (Player p in this.Players)
            {
                p.oPlayer.RecieveErrorMessage("Someone killed the game");
            }
            GameFactory.KillGame(this);
        }

        /// <summary>
        /// When someone requests an update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="playerIndex"></param>
        private void PlayerUpdateStatusRequested(object sender, EventArgs e, int playerIndex)
        {
            this.Players[playerIndex].oPlayer.UpdateRoundStatus(this.RoundStatus[playerIndex]);
        }

        /// <summary>
        /// When player (playerIndex) has played a card (e.Play)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="playerIndex"></param>
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
            // if legal, remember that the card is thrown, and update the status
            current_repeating_request = 0;
            p.Cards.Remove(e.Play);
            for( int i=0; i<4; i++)
            {
                this.RoundStatus[i].CurrentPlay[(playerIndex - i + 4)%4] = e.Play;
                this.RoundStatus[i].PlayerTurn = (PlayerSeat)((playerIndex + 1 + 4 - i) % 4);
            }
            // if first play on turn
            if (playerIndex == leadingPlayer)
            {
                roundSuit = e.Play.Suit;
                strongestCard = e.Play;
                strongestCardPlayer = playerIndex;
            }
            else
            {
                // if not check if the card is the strongest yet
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

        /// <summary>
        /// When a player gives the 3 cards to exchange
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="playerIndex"></param>
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
            // if legal perform the exchange 
            p.Cards.Remove(e.Cards[0]);
            p.Cards.Remove(e.Cards[1]);
            p.Cards.Remove(e.Cards[2]);
            // put cards to wait for target player
            int targetPlayerIndex = playerIndex + 1 + RoundStatus[0].TurnNumber;
            Player targetPlayer = this.Players[targetPlayerIndex % 4];
            targetPlayer.AwaitingCards = new List<Card>(e.Cards);
            // mark that this player gave cards
            playersReponeded |= (1 << playerIndex);
            // if all players gave cards: show all players their new cards, move to next phase
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

        /// <summary>
        /// Whan a player (playerIndex) declares his contract (e.Amount)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="playerIndex"></param>
        private void PlayerContractAccepted(object sender, RecieveContractEventArgs e, int playerIndex)
        {
            // check if contract is legal
            bool legal = false;
            if( e.Amount >= 0 && e.Amount <=13)
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
            // if legal, update the status
            totalContract += e.Amount;
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].Biddings[(playerIndex - i + 4) % 4] = new Bid { Suit = null, Amount = e.Amount };
                this.RoundStatus[i].PlayerTurn = (PlayerSeat)(((int)playerIndex + 1 - i + 4) % 4);
            }
            // update all players, ask the next player to declare
            if ((playerIndex + 1) % 4 == leadingPlayer)
            {
                // if last player, move to next phase
                StartPlaying();
            }
            else
            {
                UpdateStatusToPlayers(false);
                this.Players[(playerIndex + 1) % 4].oPlayer.RequestDeclare();
            }
        }
        
        /// <summary>
        /// When a player (playerIndex) gives his bid (e.Bid)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="playerIndex"></param>
        private void PlayerBidAccepted(object sender, RecieveBidEventArgs e, int playerIndex)
        {
            // check if bid is legal ( higher than 5, higher than current bid, or null)
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
            // if bid is null, mark this player as 'passed'
            if (e.Bid == null)
            {
                playerPassed++;
                this.Players[playerIndex].Passed = true;
            }
            else
            {
                // else mark this bid as the strongest yet
                lastBidderIndex = playerIndex;
                currentStrongBid = e.Bid;
            }
            // if all player passes, we start another frishing phase( if it's not already the last one)
            if( playerPassed == 4)
            {
                for( int i=0; i<4; i++)
                    this.RoundStatus[i].TurnNumber++;
                if( this.RoundStatus[0].TurnNumber == 3)
                {
                    // last frishing have passed, deal new cards, start again
                    StartRound();
                    return;
                }
                else
                {
                    // start new frishing
                    StartFrishing();
                    return;
                }
            }
            // move to the next player who haven't passed yet
            do{
            currentAwaitingIndex++;
            currentAwaitingIndex %= 4;
            }
            while( this.Players[currentAwaitingIndex].Passed);
            // if everyone passed but one, his bid is taken, moving to next phase
            if (currentAwaitingIndex == lastBidderIndex)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.RoundStatus[i].Trump = currentStrongBid.Value.Suit;
                    this.RoundStatus[i].LeadingPlayer = (PlayerSeat)((lastBidderIndex - i + 4) % 4);
                    this.RoundStatus[i].State = RoundState.Contract;
                }
                leadingPlayer = lastBidderIndex;
                UpdateStatusToPlayers(false);
                StartContract();
            }
            else
            {
                // if not ask, the next player to bid
                for (int i = 0; i < 4; i++)
                {
                    this.RoundStatus[i].PlayerTurn = (PlayerSeat)((currentAwaitingIndex - i + 4) % 4);
                }
                UpdateStatusToPlayers(false);
                this.Players[currentAwaitingIndex].oPlayer.RequestBid();
            }
        }

        #endregion

        #region Private Logic

        /// <summary>
        /// When the playing phase started
        /// </summary>
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

        /// <summary>
        /// When we start a playing-round
        /// </summary>
        private void StartPlayRound()
        {
            // get rid of timer
            if( tm != null)
            {
                tm.Dispose();
                tm = null;
            }
            // if we're at last playing-round end game-round
            if (this.RoundStatus[0].TurnNumber == 13)
            {
                FinishPlaying();
                return;
            }
            // update statuses, ask player leading player to start
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].TurnNumber++;
                this.RoundStatus[i].State = RoundState.Playing;
                this.RoundStatus[i].PlayerTurn = (PlayerSeat)((leadingPlayer - i + 4) % 4);
                for (int j = 0; j < 4; j++)
                    this.RoundStatus[i].CurrentPlay[j] = null;
            }
            UpdateStatusToPlayers(false);
            strongestCard = null;
            this.Players[leadingPlayer].oPlayer.RequestPlay();
        }

        /// <summary>
        /// When a playing-round is over
        /// </summary>
        private void FinishPlayRound()
        {
            // update statuses, update this playing-round winner as next playing-round leading player
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].TricksTaken[(strongestCardPlayer - i + 4) % 4]++;
                this.RoundStatus[i].LeadingPlayer = (PlayerSeat)((strongestCardPlayer - i + 4) % 4);
                
                //update roundstatus state to be "results"
                this.RoundStatus[i].State = RoundState.TurnResults;
            }
            // update players, start new playing-round with timer ( to prevent stack overflow)
            leadingPlayer = strongestCardPlayer;
            UpdateStatusToPlayers(true);
            UpdateStatusToPlayers(false);
            tm = new Timer(new TimerCallback(l => StartPlayRound()), null, Viewer != null ? milisecs_between_truns : 10, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// When the playing phase is done
        /// </summary>
        private void FinishPlaying()
        {
            // calculate score for each player
            for (int i = 0; i < 4; i++)
            {
                int score = this.RoundStatus[i].Biddings[0].Value.Amount - this.RoundStatus[i].TricksTaken[0];
                if (score == 0)
                {
                    //check if played for zero
                    if (this.RoundStatus[i].Biddings[0].Value.Amount == 0)
                    {
                        score = GetScoreForZeroBid(i, Math.Abs(score));
                    }
                    else
                    {
                        score = (int)Math.Pow(this.RoundStatus[i].TricksTaken[0], 2.0) + 10;
                    }
                }
                else
                {
                    //check if played for zero
                    if (this.RoundStatus[i].Biddings[0].Value.Amount == 0)
                    {
                        score = GetScoreForZeroBid(i, Math.Abs(score));
                    }
                    else
                    {
                        score = Math.Abs(score) * -10;
                    }
                }
                for (int j = 0; j < 4; j++)
                {
                    this.GameStatus[j].Scores[(i - j + 4) % 4] += score;
                }
            }
            
            CompleteRound();
        }

        private int GetScoreForZeroBid(int i, int score)
        {
            //check if over \ under
            if (this.RoundStatus[i].IsBiddingUnder)
            {
                return (score == 0) ? 50 : (-50 + (10 * (score-1)));
            }
            else
            {
                return (score == 0) ? 25 : (-25 + (5 * (score - 1)));
            }
        }

        /// <summary>
        /// When starting the contract phase
        /// </summary>
        private void StartContract()
        {
            UpdateStatusToPlayers(true);
            totalContract = 0;
            Player p = this.Players[leadingPlayer];
            p.oPlayer.RequestDeclare();
        }

        /// <summary>
        /// Initial function to start the game from
        /// </summary>
        private void StartGame()
        {
            // update player names and scores
            var playerNames = (from p in Players
                               select p.oPlayer.Name).ToArray();
            var playerTypes = (from p in Players
                               select p.Type).ToArray();
            for (int i = 0; i < 4; i++)
            {
                this.GameStatus[i].PlayerNames = GetArrayFrom(playerNames, i);
                this.GameStatus[i].Scores = new int[4] { 0, 0, 0, 0 };
                this.GameStatus[i].PlayerTypes = GetArrayFrom(playerTypes, i);

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

        /// <summary>
        /// Finish game-round
        /// </summary>
        private void CompleteRound()
        {
            // move dealer to next player
            this.dealerIndex++;
            this.dealerIndex %= 4;


            for (int i = 0; i < 4; i++)
            {
                this.GameStatus[i].RoundNumber++;
            }
            UpdateGameStatusToPlayers();
            // check if game is over
            if (this.GameStatus[0].RoundNumber >= num_of_rounds )
                GameFactory.KillGame(this);
            else
                StartRound();
        }

        /// <summary>
        /// When starting a game-round
        /// </summary>
        private void StartRound()
        {

            // update round status
            for (int i = 0; i < 4; i++)
            {
                this.RoundStatus[i].LeadingPlayer = (PlayerSeat)((this.dealerIndex - i + 4) % 4);
                this.RoundStatus[i].PlayerTurn = (PlayerSeat)((this.dealerIndex - i + 4) % 4);
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
                    Thread.Sleep(10);
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


        /// <summary>
        /// When starting a frishing phase
        /// </summary>
        private void StartFrishing()
        {
            playersReponeded = 0;
            foreach (Player p in Players)
            {
                p.oPlayer.RequestExhangeCards();
            }

        }

        /// <summary>
        /// When starting a bidding phase
        /// </summary>
        private void StartBidding()
        {
            for (int i = 0; i < 4; i++)
            {
                // first bidder is the dealer
                this.Players[i].Passed = false;
                this.RoundStatus[i].LeadingPlayer = (PlayerSeat)((this.dealerIndex - i + 4) % 4);
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