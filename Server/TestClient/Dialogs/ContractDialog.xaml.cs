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
    public partial class ContractDialog : UserControl, IDialogContent<int>
    {
        public ContractDialog(Suit? trump)
        {
            InitializeComponent();
            var nums = new int[14] { 0 ,1, 2,3,4,5,6,7,8,9,10,11,12,13 };
            comboBox1.ItemsSource = (from n in nums
                                     select new StamItem { Name = n.ToString(), Value = n });
            comboBox1.SelectedIndex = 0;

            this.image1.Source = new BitmapImage(new Uri("../" + ContractDialog.GetSuitImageUrl(trump), UriKind.Relative));
        }

        public static string GetSuitImageUrl(Suit? trump)
        {
            return String.Format("Images/{0}.PNG", trump.HasValue ? trump.Value.ToString() : "nosuit");
        }

        #region IDialogContent<int> Members

        public int GetValue()
        {
            return (int)((StamItem)comboBox1.SelectedItem).Value;
        }

        #endregion
    }

    public class ContractDialogClass : Dialog<int>
    {
        private ContractDialog cnt;
        public ContractDialogClass(Suit? trump)
        {
            cnt = new ContractDialog(trump);
        }
        protected override IDialogContent<int> DialogContent
        {
            get { return cnt; }
        }
    }
}
