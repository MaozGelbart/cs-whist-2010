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
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.ServiceModel;
using TestClient.GameService;
using System.Collections.ObjectModel;

namespace TestClient
{
    public partial class MainPage : UserControl
    {
 


        public MainPage()
        {
            InitializeComponent();
            MainApp.client.RecieveGameStatusUpdateReceived += new EventHandler<RecieveGameStatusUpdateReceivedEventArgs>(client_RecieveGameStatusUpdateReceived);
            MainApp.client.RecieveRoundStatusUpdateReceived += new EventHandler<RecieveRoundStatusUpdateReceivedEventArgs>(client_RecieveRoundStatusUpdateReceived);
            MainApp.client.RecieveStatusCardsReceived += new EventHandler<RecieveStatusCardsReceivedEventArgs>(client_RecieveStatusCardsReceived);
            MainApp.client.GetPlayerPlugInsCompleted += new EventHandler<GetPlayerPlugInsCompletedEventArgs>(client_GetPlayerPlugInsCompleted);
            MainApp.client.GetPlayerPlugInsAsync();
        }

        void client_RecieveStatusCardsReceived(object sender, RecieveStatusCardsReceivedEventArgs e)
        {
            if (viewer == null)
                StartView();
            var allCards = (from a in e.allCards
                            select a.ToArray()).ToArray();
            viewer.UpdateRoundStatus(e.status, allCards);
        }

        void client_GetPlayerPlugInsCompleted(object sender, GetPlayerPlugInsCompletedEventArgs e)
        {
            var types = e.Result.ToArray();
            this.Dispatcher.BeginInvoke(delegate() { LoadRegistration(types); });
        }

        public App MainApp
        {
            get
            {
                return (App)App.Current;
            }
        }

        public GameStatus gameStatus = null;

        public RoundStatus roundStatus = null;

        private Table gameTable = null;

        private ViewGame viewer = null;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void LoadRegistration(PlayerPlugin[] types)
        {
            mainPanel.Child = new Registration(types);
        }

        void client_RecieveRoundStatusUpdateReceived(object sender, RecieveRoundStatusUpdateReceivedEventArgs e)
        {
            roundStatus = e.status;
            if (gameTable != null)
            {
                gameTable.UpdateRoundStatus(roundStatus);
            }
            else
            {
                if (gameStatus != null)
               {
                   StartGame();
                }
            }
        }


        void client_RecieveGameStatusUpdateReceived(object sender, RecieveGameStatusUpdateReceivedEventArgs e)
        {
            gameStatus = e.status;
            if (gameTable != null)
            {
                gameTable.InitView(gameStatus, roundStatus);
            }
            else
            {
                if (viewer != null)
                {
                    viewer.InitView(e.status);
                }
                else
                {
                    if (roundStatus != null)
                    {
                        StartGame();
                    }
                }
            }
        }

        private void StartGame()
        {
            Registration rgt = (Registration)mainPanel.Child;
            if (rgt.dialog != null)
                rgt.dialog.Close();
            gameTable = new Table();
            gameTable.InitView(gameStatus, roundStatus);
            TestClient.App.UIThread.Run(LoadGameTable);
        }

        private void StartView()
        {
            viewer = new ViewGame();
            viewer.InitView(gameStatus);
            TestClient.App.UIThread.Run(LoadViewGameTable);
        }

        void LoadGameTable()
        {
            mainPanel.Child = gameTable;
        }



        void LoadViewGameTable()
        {
            mainPanel.Child = viewer;
        }
    }

}
