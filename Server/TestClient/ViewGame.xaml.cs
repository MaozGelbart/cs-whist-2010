using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TestClient.GameService;
using System.Windows.Media.Imaging;

namespace TestClient
{
    public partial class ViewGame : UserControl
    {
        public ViewGame()
        {
            InitializeComponent();
        }

        public void InitView(GameStatus game_status)
        {
            currentStatus = new RoundStatus();
            SetNames(game_status.PlayerNamesk__BackingField.ToArray());
            lbl_state.Content = "Bidding";
            lbl_strong_shape.Content = "";
            UpdateTakes(new[] { 0, 0, 0, 0 });
            UpdateBids(new[] { "", "", "", "" });
            UpdateScores(game_status.Scoresk__BackingField.ToArray());
            ShowCards(new Card?[4] { null, null, null, null });
        }

        public void UpdateRoundStatus(RoundStatus status, Card[][] allCards)
        {
//            if (status.Statek__BackingField != currentStatus.Statek__BackingField)
 //           {
  //              StartNewState(status.Statek__BackingField);
   //         }
            currentStatus = status;
            lbl_state.Content = status.Statek__BackingField.ToString();
            Brush red = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            Brush black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            lbl_Name0.Foreground = black;
            lbl_Name1.Foreground = black;
            lbl_Name2.Foreground = black;
            lbl_Name3.Foreground = black;
            switch (status.PlayerTurnk__BackingField)
            {
                case 0:
                    lbl_Name0.Foreground = red;
                    break;
                case 1:
                    lbl_Name1.Foreground = red;
                    break;
                case 2:
                    lbl_Name2.Foreground = red;
                    break;
                case 3:
                    lbl_Name3.Foreground = red;
                    break;
            }
            var bids = (from b in status.Biddingsk__BackingField
                        select b.HasValue ? String.Format("{0} {1}", b.Value.Amountk__BackingField, b.Value.Suitk__BackingField.ToString()) : "").ToArray();
            UpdateBids(bids);
            ShowCards(status.CurrentPlayk__BackingField.ToArray());
            lbl_strong_shape.Content = status.Trumpk__BackingField.HasValue ? status.Trumpk__BackingField.Value.ToString() : "";
            UpdateTakes(status.TricksTakenk__BackingField.ToArray());
            RecieveCards(allCards);
        }

        private void StartNewState(RoundState roundState)
        {
        }

        private void UpdateBids(string[] p)
        {
            lbl_bid0.Content = p[0];
            lbl_bid1.Content = p[1];
            lbl_bid2.Content = p[2];
            lbl_bid3.Content = p[3];
        }

        private void SetNames(string[] names)
        {
            lbl_Name0.Content = names[0];
            lbl_Name1.Content = names[1];
            lbl_Name2.Content = names[2];
            lbl_Name3.Content = names[3];
        }

        private void UpdateScores(int[] p)
        {
            lbl_score0.Content = p[0];
            lbl_score1.Content = p[1];
            lbl_score2.Content = p[2];
            lbl_score3.Content = p[3];
        }

        private void ShowCards(Card?[] cards)
        {
            if (cards[0].HasValue)
            {
                image0.Source = new BitmapImage(new Uri(GetCardImageSouce(cards[0].Value), UriKind.Relative));
                image0.Visibility = System.Windows.Visibility.Visible;
            }
            else
                image1.Visibility = System.Windows.Visibility.Collapsed;
            if (cards[1].HasValue)
            {
                image1.Source = new BitmapImage(new Uri(GetCardImageSouce(cards[1].Value), UriKind.Relative));
                image1.Visibility = System.Windows.Visibility.Visible;
            }
            else
                image1.Visibility = System.Windows.Visibility.Collapsed;
            if (cards[2].HasValue)
            {
                image2.Source = new BitmapImage(new Uri(GetCardImageSouce(cards[2].Value), UriKind.Relative));
                image2.Visibility = System.Windows.Visibility.Visible;
            }
            else
                image2.Visibility = System.Windows.Visibility.Collapsed;
            if (cards[3].HasValue)
            {
                image3.Source = new BitmapImage(new Uri(GetCardImageSouce(cards[3].Value), UriKind.Relative));
                image3.Visibility = System.Windows.Visibility.Visible;
            }
            else
                image3.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UpdateTakes(int[] takes)
        {
            lbl_takes0.Content = takes[0];
            lbl_takes1.Content = takes[1];
            lbl_takes2.Content = takes[2];
            lbl_takes3.Content = takes[3];
        }

        private void RecieveCards(Card[][] allCards)
        {
            ListBox[] lists = new ListBox[4] { lst_Cards0, lst_Cards1, lst_Cards2, lst_Cards3 };
            for (int i = 0; i < 4; i++)
            {
                var cards = allCards[i].OrderBy(c => c.Suitk__BackingField).ThenBy(c => c.Valuek__BackingField).ToList();
                var paths = (from c in cards
                             select new CardThumbnailView(GetCardImageSouce(c), c)).ToArray();
                lists[i].ItemsSource = paths;
            }
        }

        private string GetCardNumberSymbol(int value)
        {
            string num;
            if (value <= 10)
                num = value.ToString();
            else
            {
                switch (value)
                {
                    case 11:
                        num = "j";
                        break;
                    case 12:
                        num = "q";
                        break;
                    case 13:
                        num = "k";
                        break;
                    default:
                        num = "a";
                        break;
                }
            }
            return num;
        }

        private string GetCardImageSouce(Card c)
        {
            return String.Format("Images/{0}-{1}-75.png", c.Suitk__BackingField.ToString(), GetCardNumberSymbol(c.Valuek__BackingField));
        }

        public RoundStatus currentStatus { get; set; }
    }
    public class CardThumbnailView
    {
        public string path { get; set; }
        public Card card { get; set; }
        public CardThumbnailView(string path, Card card)
        {
            this.path = path;
            this.card = card;
        }
    }
}
