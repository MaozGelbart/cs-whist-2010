using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Server.API;

namespace WindowsClient
{
    public partial class GameTable : Form, IGameViewer
    {
        List<object> statusHistory = new List<object>();
        int index = -1;

        int roundIndex = 0;
        int maxRonudIndex = 0;
        Dictionary<int, int> round_to_index = new Dictionary<int, int>();
        int num_of_rounds;

        public GameTable(int num_of_rounds)
        {
            InitializeComponent();

            this.num_of_rounds = num_of_rounds;

            string[] c_id = new[]
                {
                    "2","3","4","5","6","7","8","9","10","j","q","k", "a"
                };
            string rootFolder = "../../../TestClient/Images/";
            cardImages_spades = (from id in c_id
                                 select Image.FromFile(rootFolder + "spades-" + id + "-75.png")).ToList();
            cardImages_hearts = (from id in c_id
                                 select Image.FromFile(rootFolder + "hearts-" + id + "-75.png")).ToList();
            cardImages_diamonds = (from id in c_id
                                   select Image.FromFile(rootFolder + "diamonds-" + id + "-75.png")).ToList();
            cardImages_clubs = (from id in c_id
                                select Image.FromFile(rootFolder + "clubs-" + id + "-75.png")).ToList();

            this.KeyPress += new KeyPressEventHandler(GameTable_KeyPress);
        }


        void GameTable_KeyPress(object sender, KeyPressEventArgs e)
         {
            if (e.KeyChar == 28)
            {
                index--;
                ShowStatus();
            }
            if (e.KeyChar == 26)
            {
                index++;
                ShowStatus();
            }
        }


        #region IGameViewer Members

        public void UpdateGameStatus(GameStatus status)
        {
            //BeginInvoke(new MethodInvoker( delegate() { ShowGameStatus(status); }));
            statusHistory.Add(status.Clone());
            int _roundIndex = status.RoundNumber;
            if (!round_to_index.ContainsKey(_roundIndex))
                round_to_index.Add(_roundIndex, statusHistory.Count - 1);
            if (_roundIndex > maxRonudIndex)
                maxRonudIndex = _roundIndex;
            BeginInvoke(new MethodInvoker(delegate() { lbl_status.Text = "Finished simulating round #" + maxRonudIndex; }));
            
        }

        public void UpdateRoundStatus(RoundStatus status, Card[][] allCards)
        {
            //BeginInvoke(new MethodInvoker( delegate() { ShowRoundStatus(status, allCards); }));
            statusHistory.Add(new status_and_cards{ Status = status.Clone(), Cards = (Card[][])allCards.Clone() });
        }

        public event EventHandler<EventArgs> OnKillGameRequested;

        public void RecieveErrorMessage(string msg)
        {
            BeginInvoke(new MethodInvoker(delegate() { lbl_message.Text = msg; }));
        }

        public void RecieveGameOver()
        {
            BeginInvoke(new MethodInvoker(delegate() { lbl_message.Text = "Game Over"; }));
        }

        #endregion

        private void ShowStatus()
        {
            if (index < 0)
            {
                index = 0;
                return;
            }
            if (index >= statusHistory.Count)
            {
                index = statusHistory.Count - 1;
                return;
            }
            object status = statusHistory[index];
            if (status is status_and_cards)
                ShowRoundStatus(((status_and_cards)status).Status, ((status_and_cards)status).Cards);
            else
                ShowGameStatus((GameStatus)status);
        }

        private void ShowGameStatus(GameStatus status)
        {
            lbl_name_self.Text = status.PlayerNames[0];
            lbl_name_west.Text = status.PlayerNames[1];
            lbl_name_north.Text = status.PlayerNames[2];
            lbl_name_east.Text = status.PlayerNames[3];


            lbl_score_self.Text = status.Scores[0] + "";
            lbl_score_west.Text = status.Scores[1] + "";
            lbl_score_north.Text = status.Scores[2] + "";
            lbl_score_east.Text = status.Scores[3] + "";

            lbl_round_count.Text = status.RoundNumber + "";
            ShowRoundStatus(new RoundStatus { Biddings = new Bid?[4], CurrentPlay = new Card?[4], TricksTaken = new int[4]}, new[] { new Card[0], new Card[0], new Card[0], new Card[0] });
        }

        public void ShowRoundStatus(RoundStatus status, Card[][] allCards)
        {
            lbl_trump.Text = status.Trump.HasValue ? status.Trump.Value.ToString() : "No-suit" ;

            lbl_bid_self.Text = status.Biddings[0] + "";
            lbl_bid_west.Text = status.Biddings[1] + "";
            lbl_bid_north.Text = status.Biddings[2] + "";
            lbl_bid_east.Text = status.Biddings[3] + "";

            lbl_tricks_self.Text = status.TricksTaken[0] + "";
            lbl_tricks_west.Text = status.TricksTaken[1] + "";
            lbl_tricks_north.Text = status.TricksTaken[2] + "";
            lbl_tricks_east.Text = status.TricksTaken[3] + "";

            SetCardImage(play_self, status.CurrentPlay[0]);
            SetCardImage(play_west, status.CurrentPlay[1]);
            SetCardImage(play_north, status.CurrentPlay[2]);
            SetCardImage(play_east, status.CurrentPlay[3]);

            SetAllCardsToPlayer(cards_self, allCards[0]);
            SetAllCardsToPlayer(cards_west, allCards[1]);
            SetAllCardsToPlayer(cards_north, allCards[2]);
            SetAllCardsToPlayer(cards_east, allCards[3]);
        }


        private void SetAllCardsToPlayer(Panel list, Card[] card)
        {
            list.Controls.Clear();
            int y = 0;
            int x = 0;
            var arranged = (from c in card
                            orderby (c.Suit != Suit.Hearts ? (int)c.Suit : 5), c.Value
                            select c).ToArray();
            foreach (Card c in arranged)
            {
                PictureBox box = new PictureBox();
                box.Width = 36;
                box.Height = 46;
                box.Location = new Point(x, y);
                if (list.Height > 100)
                    y += 50;
                else
                    x += 38;
                box.SizeMode = PictureBoxSizeMode.StretchImage;
                box.Image = GetImageForCard(c);
                list.Controls.Add(box);
            }
        }

        private void SetCardImage(PictureBox play_self, Card? card)
        {
            if (card.HasValue)
            {
                play_self.Image = GetImageForCard(card.Value);
            }
            else
                play_self.Image = null;
        }

        List<Image> cardImages_spades;
        List<Image> cardImages_hearts;
        List<Image> cardImages_diamonds;
        List<Image> cardImages_clubs;

        private List<Image> GetImageList(Suit s)
        {
            switch (s)
            {
                case Suit.Clubs:
                    return cardImages_clubs;
                case Suit.Hearts:
                    return cardImages_hearts;
                case Suit.Diamonds:
                    return cardImages_diamonds;
                case Suit.Spades:
                default:
                    return cardImages_spades;
            }
        }

        private Image GetImageForCard(Card card)
        {
            return GetImageList(card.Suit)[card.Value-2];
        }

        private class status_and_cards
        {
            public RoundStatus Status { get; set; }
            public Card[][] Cards { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            index = 0;
            ShowStatus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            roundIndex = maxRonudIndex;
            index = round_to_index[roundIndex];
            ShowStatus();

            index = statusHistory.Count - 1;
            ShowStatus();
        }

        private void btn_prev_turn_Click(object sender, EventArgs e)
        {
            index--;
            ShowStatus();
        }

        private void btn_next_turn_Click(object sender, EventArgs e)
        {
            index++;
            ShowStatus();
        }

        private void btn_prev_round_Click(object sender, EventArgs e)
        {
            roundIndex--;
            if (roundIndex < 0)
                roundIndex = 0;
            index = round_to_index[roundIndex];
            ShowStatus();
        }

        private void btn_next_round_Click(object sender, EventArgs e)
        {
            roundIndex++;
            if (roundIndex > maxRonudIndex)
                roundIndex = maxRonudIndex;
            index = round_to_index[roundIndex];
            ShowStatus();
        }

    }
}
