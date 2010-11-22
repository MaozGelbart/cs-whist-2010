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
using System.Collections.ObjectModel;
using TestClient.Dialogs;
using TestClient.GameService;

namespace TestClient
{
    public partial class Registration : UserControl
    {
        List<PlayerPlugin> types;
        public Registration(PlayerPlugin[] _types)
        {
            InitializeComponent();
            this.types = new List<PlayerPlugin>(_types);
            this.types.Insert(0, new PlayerPlugin() { ID="",Name= "Human"});
            PlayerType0.ItemsSource = types;
            PlayerType2.ItemsSource = types;
            PlayerType3.ItemsSource = types;
            PlayerType4.ItemsSource = types;
            PlayerType0.SelectedIndex = 0;
            PlayerType2.SelectedIndex = 1;
            PlayerType3.SelectedIndex = 1;
            PlayerType4.SelectedIndex = 1;
        }

        public App MainApp
        {
            get
            {
                return (App)App.Current;
            }
        }

        public MessageDialogClass dialog;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MainApp.client.StartGameCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_StartGameCompleted);
            MainApp.client.StartGameViewCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_StartGameCompleted);
            System.Collections.ObjectModel.ObservableCollection<string> lst = new System.Collections.ObjectModel.ObservableCollection<string>();
            if (PlayerType0.SelectedIndex > 0)
            {
                lst.Add(((PlayerPlugin)PlayerType0.SelectedItem).ID);
            }
            if (PlayerType2.SelectedIndex > 0)
            {
                lst.Add(((PlayerPlugin)PlayerType2.SelectedItem).ID);
            }
            if (PlayerType3.SelectedIndex > 0)
            {
                lst.Add(((PlayerPlugin)PlayerType3.SelectedItem).ID);
            }
            if (PlayerType4.SelectedIndex > 0)
            {
                lst.Add(((PlayerPlugin)PlayerType4.SelectedItem).ID);
            }
            if (PlayerType0.SelectedIndex == 0)
            {
                MainApp.client.StartGameAsync(PlayerName.Text, lst.Count, lst);
            }
            else
                MainApp.client.StartGameViewAsync(lst);
            if (lst.Count < 3)
            {
                dialog = new MessageDialogClass("Waiting for players");
                dialog.Show(DialogStyle.Modal);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            MainApp.client.RegisterAsync(PlayerName.Text);
        }

        private void client_StartGameCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }

    }

}
