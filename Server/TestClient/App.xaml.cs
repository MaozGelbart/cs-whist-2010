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
using System.ServiceModel;
using TestClient.GameService;

namespace TestClient
{
    public partial class App : Application
    {

        public enum ViewMode
        {
            Normal = 0,
            Facebook = 1
        }

        public static class UIThread
        {
            public static System.Windows.Threading.Dispatcher Dispatcher { get; set; }
            public static void Run(Action a)
            {
                Dispatcher.BeginInvoke(a);
            }

        }

        PollingDuplexHttpBinding binding = new PollingDuplexHttpBinding();
        public Uri SERVICE_ADDRESS;
        string serviceAddress;
        EndpointAddress add;
        public PlayerServiceClient client;

        public ViewMode viewMode;
        public Account account;
        public string firstName;
        public string photoUrl;

        public App()
        {
            SERVICE_ADDRESS = System.Windows.Browser.HtmlPage.Document.DocumentUri;
            serviceAddress = "http://" + SERVICE_ADDRESS.Host + ":" + SERVICE_ADDRESS.Port + "/Wist/PlayerService.svc";
            add = new EndpointAddress(serviceAddress);
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;
            InitializeComponent();
            client = new PlayerServiceClient(binding, add);
        }

        private void InitFacebookLogin(string id)
        {
            viewMode = ViewMode.Facebook;
            client.RegisterFacebookAccountCompleted += new EventHandler<RegisterFacebookAccountCompletedEventArgs>(client_RegisterFacebookAccountCompleted);
            client.RegisterFacebookAccountAsync(id);

        }

        void client_RegisterFacebookAccountCompleted(object sender, RegisterFacebookAccountCompletedEventArgs e)
        {
            account = e.Result;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
           this.RootVisual = new MainPage();
           if (e.InitParams.ContainsKey("viewMode"))
               this.viewMode = e.InitParams["viewMode"] == "facebook" ? ViewMode.Facebook : ViewMode.Normal;
           if (this.viewMode == ViewMode.Facebook)
           {
               if (e.InitParams.ContainsKey("uid"))
               {
                   string id = e.InitParams["uid"];
                   InitFacebookLogin(id);
               }
               if (e.InitParams.ContainsKey("firstName"))
               {
                   firstName = e.InitParams["firstName"];
               }
               if (e.InitParams.ContainsKey("photo"))
               {
                   photoUrl = e.InitParams["photo"];
               }
           }
            App.UIThread.Dispatcher = RootVisual.Dispatcher;
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            client.FinishGameAsync();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
