﻿using Aimer.SDK;
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
        private DispatcherTimer mTimer2 = new DispatcherTimer();

        string request;
        private string Ip;
        Windows.Networking.Sockets.StreamSocket socket;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded +=  (s, e) =>
             {
                 frame.Navigate(typeof(VerticalPage));
                 
                
             };
            frame.Navigated += Frame_Navigated;
            mTimer2.Tick += MTimer2_Tick;
            mTimer2.Interval = TimeSpan.FromSeconds(2);
            IP.GotFocus += IP_GotFocus;
            IP.LostFocus += IP_LostFocus;
        }

        private void IP_LostFocus(object sender, RoutedEventArgs e)
        {
            mTimer.Start();
        }

        private void IP_GotFocus(object sender, RoutedEventArgs e)
        {
            mTimer2.Stop();
        }

        private void MTimer2_Tick(object sender, object e)
        {
            //serverIP.Visibility = Visibility.Visible;
            barcodeInput.Focus(FocusState.Keyboard);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            LocalIpAsync();
        }

        private async void LocalIpAsync()
        {
            
            try
            {
                var settings = ApplicationData.Current.LocalSettings;
                object value = settings.Values["IP"];
                string IPValue = value.ToString();
                IP.Text = IPValue;
                mTimer2.Start();
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
                
                
            }
            catch (Exception e)
            {
                connectbutton.Visibility = Visibility.Visible;
                stackPanel.Visibility = Visibility.Visible;
                serverIP.Visibility = Visibility.Visible;
                VerticalPage verticalpage = frame.Content as VerticalPage;
                verticalpage.hideImg();
                
            }
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
                if(len== 40||len==69)
                {
                    serverDisposed.Visibility = Visibility.Visible;
                }
                
            }
        }

        private async void connectbutton_ClickAsync(object sender, RoutedEventArgs e)
        {
            mTimer2.Start();
            TextBox TBox = IP;
            Ip = TBox.Text;
            var settings  = ApplicationData.Current.LocalSettings;
            settings.Values["IP"] = Ip;
            serverDisposed.Visibility = Visibility.Collapsed;
            try
            {
                socket = new Windows.Networking.Sockets.StreamSocket();
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(Ip);
                string serverPort = "1337";
                await socket.ConnectAsync(serverHost, serverPort);
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                Writer = new StreamWriter(streamOut);
                VerticalPage verticalpage = frame.Content as VerticalPage;
                verticalpage.AppearImg();
                //create timer
                mTimer.Tick += MTimer_Tick;
                mTimer.Interval = TimeSpan.FromSeconds(5);
                mTimer.Start();

                connectbutton.Visibility = Visibility.Collapsed;
                stackPanel.Visibility = Visibility.Collapsed;
                serverIP.Visibility = Visibility.Collapsed;
                
            }
            catch (Exception ex)
            {
                
            }
           
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
        public string getText()
        {
            string text = barcodeInput.Text;
            barcodeInput.Text = "";
            return text;
        }
    }
}
