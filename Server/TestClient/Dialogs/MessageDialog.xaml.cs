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

namespace TestClient.Dialogs
{
    public partial class MessageDialog : UserControl, IDialogContent<object>
    {
        public MessageDialog()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        #region IDialogContent<object> Members

        public object GetValue()
        {
            return null;
        }

        #endregion
    }

    public class MessageDialogClass : Dialog<object>
    {
        MessageDialog cnt;
        public MessageDialogClass(string text)
        {
            cnt = new MessageDialog();
            cnt.Text = text;
        }

        protected override IDialogContent<object> DialogContent
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
