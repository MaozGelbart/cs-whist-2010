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
using TestClient.Dialogs;

namespace TestClient
{
    public partial class Table : UserControl
    {
        public Table()
        {
            InitializeComponent();
            MainApp.client.RecieveCardsReceived += new EventHandler<RecieveCardsReceivedEventArgs>(client_RecieveCardsReceived);
            MainApp.client.RequestExchangeCardsReceived += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_RequestExchangeCardsReceived);
            MainApp.client.RecieveExchangedCardsReceived += new EventHandler<RecieveExchangedCardsReceivedEventArgs>(client_RecieveExchangedCardsReceived);
            MainApp.client.RequestBidReceived += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_RequestBidReceived);
            MainApp.client.ReqeustContractReceived += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_ReqeustContractReceived);
            MainApp.client.RequestPlayReceived += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_RequestPlayReceived);
        }

        List<Card> cards;

        App MainApp
        {
            get
            {
                return (App)App.Current;
            }
        }

        RoundStatus currentStatus;
        private bool exchangedCardRecieved;
        private bool bidRequested;

        #region Web Client Callbacks

        void client_RequestPlayReceived(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageDialogClass dialog = new MessageDialogClass("Select a card to play");
            dialog.Show(DialogStyle.Modal);
            TestClient.App.UIThread.Run(SwitchCardSelectionToSingle);
        }

        void client_ReqeustContractReceived(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ContractDialogClass dialog = new ContractDialogClass(currentStatus.Trumpk__BackingField);
            dialog.OnResponse += new EventHandler<DialogEventArgs<int>>(dialog_OnResponse);
            dialog.Show(DialogStyle.Modal);
        }

        void client_RequestBidReceived(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (exchangedCardRecieved)
            {
                DialogBidClass dialog = new DialogBidClass();
                dialog.OnResponse += new EventHandler<DialogEventArgs<Bid?>>(dialog_OnResponse);
                dialog.Show(DialogStyle.Modal);
            }
            else
                bidRequested = true;
        }

        void client_RecieveExchangedCardsReceived(object sender, RecieveExchangedCardsReceivedEventArgs e)
        {
            DialogExchangedCardsClass dialog = new DialogExchangedCardsClass(e.cards[0], e.cards[1], e.cards[2]);
            exchangedCardRecieved = false;
            bidRequested = true;
            dialog.OnResponse += new EventHandler<DialogEventArgs<bool>>(dialog_OnResponse);
            dialog.Show(DialogStyle.Modal);
            cards.AddRange(e.cards);
            RecieveCards();
        }

        void dialog_OnResponse(object sender, DialogEventArgs<bool> e)
        {
            exchangedCardRecieved = true;
            if (bidRequested)
            {
                DialogBidClass dialog = new DialogBidClass();
                dialog.OnResponse += new EventHandler<DialogEventArgs<Bid?>>(dialog_OnResponse);
                dialog.Show(DialogStyle.Modal);
            }
        }

        void client_RequestExchangeCardsReceived(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string dir = currentStatus.TurnNumberk__BackingField == 0 ? "left" : (currentStatus.TurnNumberk__BackingField == 1 ? "forward" : "right");
            MessageDialogClass dialog = new MessageDialogClass("Please choose 3 cards to pass to the " + dir + " player");
            dialog.Show(DialogStyle.Modal);
            TestClient.App.UIThread.Run(SwitchCardSelectionToMultiple);
        }
        void client_RecieveCardsReceived(object sender, RecieveCardsReceivedEventArgs e)
        {
            cards = e.cards.ToList();
            RecieveCards();
        }

        #endregion

        #region Gui Thread delegates

        void SwitchCardSelectionToMultiple()
        {
            lst_MyCards.SelectionMode = SelectionMode.Multiple;
            btn_ThrowCard.IsEnabled = true;
            btn_ThrowCard.Content = "Pass";
        }

        void SwitchCardSelectionToSingle()
        {
            lst_MyCards.SelectionMode = SelectionMode.Single;
            btn_ThrowCard.IsEnabled = true;
            btn_ThrowCard.Content = "Play";
        }

        #endregion

        #region Public Methods

        public void InitView(GameStatus game_status, RoundStatus status)
        {
            currentStatus = status;
            SetNames(game_status.PlayerNamesk__BackingField.ToArray());
            lbl_state.Content = "Bidding";
            UpdateTakes(new[] { 0, 0, 0, 0 });
            UpdateBids(new[] { "", "", "", "" });
            UpdateScores(game_status.Scoresk__BackingField.ToArray());
            ShowCards(new Card?[4] { null, null, null, null });
        }

        public void UpdateRoundStatus(RoundStatus status)
        {
            if (status.Statek__BackingField != currentStatus.Statek__BackingField)
            {
                StartNewState(status.Statek__BackingField, status);
            }
            currentStatus = status;
            Brush red = new SolidColorBrush(Color.FromArgb(255,255,0,0));
            Brush black = new SolidColorBrush(Color.FromArgb(255,0,0,0));
            lbl_Name0.Foreground = black;
            lbl_Name1.Foreground = black;
            lbl_Name2.Foreground = black;
            lbl_Name3.Foreground = black;
            switch (status.PlayerTurnk__BackingField)
            {
                case PlayerSeat.Self:
                    lbl_Name0.Foreground = red;
                    break;
                case PlayerSeat.West:
                    lbl_Name1.Foreground = red;
                    break;
                case PlayerSeat.North:
                    lbl_Name2.Foreground = red;
                    break;
                case PlayerSeat.East:
                    lbl_Name3.Foreground = red;
                    break;
            }
            var bids = (from b in status.Biddingsk__BackingField
                        select b.HasValue ? String.Format("{0} {1}", b.Value.Amountk__BackingField, b.Value.Suitk__BackingField.ToString()) : "" ).ToArray();
            UpdateBids(bids);
            ShowCards(status.CurrentPlayk__BackingField.ToArray());
            UpdateTakes(status.TricksTakenk__BackingField.ToArray());
        }

        #endregion

        #region Private Helpers

        private void StartNewState(RoundState roundState, RoundStatus status)
        {
            switch(roundState)
            {
                case RoundState.Contract:
                    img_trump.Source = new BitmapImage(new Uri(ContractDialog.GetSuitImageUrl(status.Trumpk__BackingField), UriKind.Relative));
                    img_trump.Visibility = System.Windows.Visibility.Visible;
                    break;
                case RoundState.Bidding:
                    img_trump.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
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
            ShowMyCard(cards[0]);
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

        private void ShowMyCard(Card? card)
        {
            if (card.HasValue)
            {
                image0.Source = new BitmapImage(new Uri(GetCardImageSouce(card.Value), UriKind.Relative));
                image0.Visibility = System.Windows.Visibility.Visible;
            }
            else
                image0.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UpdateTakes(int[] takes)
        {
            lbl_takes0.Content = takes[0];
            lbl_takes1.Content = takes[1];
            lbl_takes2.Content = takes[2];
            lbl_takes3.Content = takes[3];
        }

        private void RecieveCards()
        {
            cards = cards.OrderBy(c => c.Suitk__BackingField).ThenBy(c => c.Valuek__BackingField).ToList();
            var paths = (from c in cards
                         select new CardThumbnail(GetCardImageSouce(c), c)).ToArray();
            lst_MyCards.ItemsSource = paths;
        }

        public static string GetCardNumberSymbol(int value)
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

        public static string GetCardImageSouce(Card c)
        {
            return String.Format("Images/{0}-{1}-75.png", c.Suitk__BackingField.ToString(), GetCardNumberSymbol(c.Valuek__BackingField));
        }

        #endregion

        #region User Events

        void dialog_OnResponse(object sender, DialogEventArgs<int> e)
        {
            MainApp.client.MakeContractAsync(e.Value);
        }

        void dialog_OnResponse(object sender, DialogEventArgs<Bid?> e)
        {
            MainApp.client.MakeBidAsync(e.Value);
        }

        private void btn_ThrowCard_Click(object sender, RoutedEventArgs e)
        {
            if ((string)btn_ThrowCard.Content == "Pass")
            {
                if (lst_MyCards.SelectedItems.Count != 3)
                    MessageBox.Show("3 cards, idiot!", "Frishing", MessageBoxButton.OK);
                else
                {
                    System.Collections.ObjectModel.ObservableCollection<Card> lst = new System.Collections.ObjectModel.ObservableCollection<Card>();
                    lst.Add(((CardThumbnail)lst_MyCards.SelectedItems[0]).card);
                    lst.Add(((CardThumbnail)lst_MyCards.SelectedItems[1]).card);
                    lst.Add(((CardThumbnail)lst_MyCards.SelectedItems[2]).card);
                    MainApp.client.SwitchCardsAsync(lst);
                    btn_ThrowCard.IsEnabled = false;
                    cards.Remove(((CardThumbnail)lst_MyCards.SelectedItems[0]).card);
                    cards.Remove(((CardThumbnail)lst_MyCards.SelectedItems[1]).card);
                    cards.Remove(((CardThumbnail)lst_MyCards.SelectedItems[2]).card);
                    RecieveCards();
                }
            }
            else
            {
                if ((string)btn_ThrowCard.Content == "Play")
                {
                    Card c = ((CardThumbnail)lst_MyCards.SelectedItem).card;
                    MainApp.client.PlayCardAsync(c);
                    cards.Remove(c);
                    RecieveCards();
                    btn_ThrowCard.IsEnabled = false;
                   
                }
            }
        }

        #endregion

    }
    public class CardThumbnail
    {
        public string path { get; set; }
        public Card card { get; set; }
        public CardThumbnail(string path, Card card)
        {
            this.path = path;
            this.card = card;
        }
    }
}
