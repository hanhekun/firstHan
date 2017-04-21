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
        string request;
        private string Ip;
        Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded +=  (s, e) =>
             {
                 frame.Navigate(typeof(VerticalPage));
             };
            LocalIpAsync();
        }

        private async void LocalIpAsync()
        {
            
            try
            {
                var settings = ApplicationData.Current.LocalSettings;
                object value = settings.Values["IP"];
                string IPValue = value.ToString();
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(IPValue);
                string serverPort = "1337";
                await socket.ConnectAsync(serverHost, serverPort);
                ConnectSucess.Visibility = Visibility;
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                Writer = new StreamWriter(streamOut);
            }
            catch (Exception ex)
            {
                connectbutton.Visibility = Visibility.Visible;
                stackPanel.Visibility = Visibility.Visible;
                serverIP.Visibility = Visibility.Visible;
            }
        }


        private async void red_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            request = btn.Name;
            //Write data to the echo server.
            Stream streamOut = socket.OutputStream.AsStreamForWrite();
            StreamWriter writer = new StreamWriter(streamOut);
            await writer.WriteLineAsync(request);
            await writer.FlushAsync();
        }
        
        private async void connectbutton_ClickAsync(object sender, RoutedEventArgs e)
        {
            TextBox TBox = IP;
            Ip = TBox.Text;
            var settings  = ApplicationData.Current.LocalSettings;
            settings.Values["IP"] = Ip;
            try
            {
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(Ip);

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 1337.
                string serverPort = "1337";
                await socket.ConnectAsync(serverHost, serverPort);
                ConnectSucess.Visibility = Visibility;
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                Writer = new StreamWriter(streamOut);
            }
            catch (Exception ex)
            {

            }
           
        }

       

    }
}
