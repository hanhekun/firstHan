using Aimer.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Converters;
using System.Data;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using MetroLog;
using Windows.Storage;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Aimer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        public static StreamWriter Writer;
        private DispatcherTimer mTimer = new DispatcherTimer();
        private DispatcherTimer logoTimer = new DispatcherTimer();
        private DispatcherTimer focusTimer = new DispatcherTimer();
        private DispatcherTimer errorTimer = new DispatcherTimer();

        string request;
        private string Ip;
        private int errorTime=0;
        Windows.Networking.Sockets.StreamSocket socket;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded +=  (s, e) =>
             {
                 frame.Navigate(typeof(VerticalPage));
             };
            logoTimer.Tick += logoTimer_Tick;
            logoTimer.Interval = TimeSpan.FromSeconds(600);
            frame.Navigated += Frame_Navigated;
            focusTimer.Tick += FocusTimer_Tick;
            focusTimer.Interval = TimeSpan.FromSeconds(2);
            focusTimer.Start();
            errorTimer.Tick += ErrorTimer_Tick;
            errorTimer.Interval = TimeSpan.FromSeconds(9);
        }

        private void ErrorTimer_Tick(object sender, object e)
        {
            errorTime++;
            LocalIpAsync();
        }

        private void FocusTimer_Tick(object sender, object e)
        {
            text.Focus(FocusState.Keyboard);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            LocalIpAsync();
        }
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            DisLogo();
        }
        public void DisLogo()
        {
            logoImg.Visibility = Visibility.Collapsed;
            logoTimer.Start();
        }
        private async void LocalIpAsync()
        {
            
            try
            {
                var settings = ApplicationData.Current.LocalSettings;
                string IPValue="";
                try
                {
                    object value = settings.Values["IP"];
                    IPValue = value.ToString();
                }catch(Exception e)
                {
                    connectbutton.Visibility = Visibility.Visible;
                    stackPanel.Visibility = Visibility.Visible;
                    serverIP.Visibility = Visibility.Visible;
                    VerticalPage verticalpage = frame.Content as VerticalPage;
                    verticalpage.hideImg();
                }
                
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(IPValue);
                string serverPort = "1337";
                socket = new Windows.Networking.Sockets.StreamSocket();
                await socket.ConnectAsync(serverHost, serverPort);
                Stream streamOut = socket.OutputStream.AsStreamForWrite();              
                Writer = new StreamWriter(streamOut);
                //create timer
                mTimer.Tick += MTimer_Tick;
                mTimer.Interval = TimeSpan.FromSeconds(5);
                mTimer.Start();
                //read data from server
                Stream streamIn = socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(streamIn);
                if (reader != null)
                {
                    connectFalse.Visibility = Visibility.Visible;
                    connectFalse.Content = reader;
                }
                errorTime = 0;
            }
            catch (Exception e)
            {
                errorTimer.Start();
                if (errorTime > 40)
                {
                    connectbutton.Visibility = Visibility.Visible;
                    stackPanel.Visibility = Visibility.Visible;
                    serverIP.Visibility = Visibility.Visible;
                    VerticalPage verticalpage = frame.Content as VerticalPage;
                    verticalpage.hideImg();
                }
            }
        }
        private async void logoTimer_Tick(object sender, object e)
        {
            logoImg.Visibility = Visibility.Visible;
        }
        private async void MTimer_Tick(object sender, object e)
        {
            try
            {
                await Writer.WriteLineAsync("testConnect");
                await Writer.FlushAsync();
            }
            catch (Exception ex)
            {
                int len = ex.Message.Length;
                if (len == 40)
                {
                   // serverDisposed.Visibility = Visibility.Visible;
                    VerticalPage verticalpage = frame.Content as VerticalPage;
                    verticalpage.HideButton();
                }
            }
        }
        public async System.Threading.Tasks.Task SenderSceneAsync(string str)
        {
            try
            {
                await Writer.WriteLineAsync(str);
                await Writer.FlushAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private async void connectbutton_ClickAsync(object sender, RoutedEventArgs e)
        {
            VerticalPage verticalpage = frame.Content as VerticalPage;
            errorTime = 0;

            serverDisposed.Visibility = Visibility.Collapsed;
            TextBox TBox = IP;
            Ip = TBox.Text;
            var settings  = ApplicationData.Current.LocalSettings;
            settings.Values["IP"] = Ip;
            try
            {
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(Ip);
                string serverPort = "1337";
                socket = new Windows.Networking.Sockets.StreamSocket();
                await socket.ConnectAsync(serverHost, serverPort);
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                Writer = new StreamWriter(streamOut);
                verticalpage = frame.Content as VerticalPage;
                verticalpage.AppearImg();
                mTimer.Tick += MTimer_Tick;
                mTimer.Interval = TimeSpan.FromSeconds(5);
                mTimer.Start();

                serverIP.Visibility = Visibility.Collapsed;
                connectbutton.Visibility = Visibility.Collapsed;
                stackPanel.Visibility = Visibility.Collapsed;
                serverIP.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                
            }
           
        }
        public string getText()
        {
            string t = text.Text;
            text.Text = "";
            return t;
        }

        private void serverDisposed_Click(object sender, RoutedEventArgs e)
        {
            serverDisposed.Visibility = Visibility.Collapsed;
            connectbutton.Visibility = Visibility.Visible;
            stackPanel.Visibility = Visibility.Visible;
            serverIP.Visibility = Visibility.Visible;

            VerticalPage verticalpage = frame.Content as VerticalPage;
            verticalpage.hideImg();

        }
    }
}
