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
using System.Windows.Media.Imaging;
using TestClient.GameService;

namespace TestClient.Dialogs
{
    public partial class DialogExchangedCards : UserControl, IDialogContent<bool>
    {
        public DialogExchangedCards(Card card0, Card card1, Card card2)
        {
            InitializeComponent();

            this.img_card0.Source = new BitmapImage(new Uri("../" + Table.GetCardImageSouce(card0), UriKind.Relative));

            this.img_card1.Source = new BitmapImage(new Uri("../" + Table.GetCardImageSouce(card1), UriKind.Relative));

            this.img_card2.Source = new BitmapImage(new Uri("../" + Table.GetCardImageSouce(card2), UriKind.Relative));

        }

        #region IDialogContent<bool> Members

        public bool GetValue()
        {
            return true;
        }

        #endregion
    }

    public class DialogExchangedCardsClass : Dialog<bool>
    {
        DialogExchangedCards cnt;
        public DialogExchangedCardsClass(Card card0, Card card1, Card card2)
        {
            cnt = new DialogExchangedCards(card0, card1, card2);
        }
        protected override IDialogContent<bool> DialogContent
        {
            get { return cnt; }
        }
        protected override void OnClickOutside()
        {
            base.OnClickOutside();
            Close();
        }
    }
}
