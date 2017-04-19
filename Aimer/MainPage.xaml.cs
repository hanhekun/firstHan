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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Aimer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        string request;
        Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) =>
             {
                 Log.Info("Loaded");
                 //frame.Navigate(typeof(VerticalPage));
             };
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

        private async void blue_Click(object sender, RoutedEventArgs e)
        {
           
            Button btn = (Button)sender;
            request = btn.Name;
            //Write data to the echo server.
            Stream streamOut = socket.OutputStream.AsStreamForWrite();
            StreamWriter writer = new StreamWriter(streamOut);
            await writer.WriteLineAsync(request);
            await writer.FlushAsync();

        }

        private async void green_Click(object sender, RoutedEventArgs e)
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
            var Ip = TBox.Text;
            
            try
            {
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(Ip);

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 1337.
                string serverPort = "1337";
                await socket.ConnectAsync(serverHost, serverPort);
            }
            catch (Exception ex)
            {

            }
            ConnectSucess.Visibility = Visibility;

            blue.IsEnabled = true;
            red.IsEnabled = true;
            yellow.IsEnabled = true;   
           
        }

       

    }
}
