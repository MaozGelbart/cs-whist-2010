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

namespace TestClient
{
    public partial class DialogBid : UserControl, IDialogContent<Bid?>
    {
        public DialogBid()
        {
            InitializeComponent();
            var suits = new Suit? [5] { Suit.Spades, Suit.Hearts, Suit.Diamonds, Suit.Clubs, null };
            var nums = new int[15] { -1,0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            lst_suits.ItemsSource = (from s in suits
                                     select new StamItem { Name = s.HasValue ? s.ToString() : "None", Value = s }).ToArray();
            lst_amounts.ItemsSource = (from n in nums
                                       select new StamItem { Name = n> -1 ? n.ToString() : "Pass", Value = n }).ToArray();
            lst_suits.SelectedIndex = 0;
            lst_amounts.SelectedIndex = 0;
        }

        #region IDialogContent Members

        public Bid? GetValue()
        {
            Suit? s = (Suit?)((StamItem)lst_suits.SelectedItem).Value;
            int amount = (int)((StamItem)lst_amounts.SelectedItem).Value;
            if (amount > -1)
                return new Bid { Suitk__BackingField = s, Amountk__BackingField = amount };
            else
                return null;
        }

        #endregion
    }

}
