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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Aimer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string request;
        Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();
        public MainPage()
        {
            this.InitializeComponent();
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

            //Read data from the echo server.
            Stream streamIn = socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn);
            string response = await reader.ReadLineAsync();
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

            //Read data from the echo server.
            Stream streamIn = socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn);
            string response = await reader.ReadLineAsync();
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

            //Read data from the echo server.
            Stream streamIn = socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn);
            string response = await reader.ReadLineAsync();
        }
        
        private async void connectbutton_ClickAsync(object sender, RoutedEventArgs e)
        {
            TextBox TBox = IP;
            var Ip = TBox.Text;
            
            try
            {
                Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();

                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(Ip);

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 1337.
                string serverPort = "1337";
                await socket.ConnectAsync(serverHost, serverPort);
            }
            catch (Exception ex)
            {

            }


            blue.IsEnabled = true;
            red.IsEnabled = true;
            yellow.IsEnabled = true;   
           
        }

        //private  void ScenButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button Btn = (Button)sender;

        //    var BtnName=Btn.Name.Remove(0, 6);

        //    ImgButtonPanel.Margin = new Thickness(96, 192, 192, 96);

        //    for (int i = 0; i < data.scenslist.Count; i++)
        //    {

        //        if (data.scenslist.Keys.ElementAt(i).ToString()== BtnName)
        //        {
        //            var key = data.scenslist.Keys.ElementAt(i);
        //            var value= data.scenslist[key];
        //            var Path1 = value.imgpath1;
        //            var Path2 = value.imgpath2;
        //            var Path3 = value.imgpath3;
        //            Image img1 = new Image();
        //            Image img2 = new Image();
        //            Image img3 = new Image();
        //            BitmapImage bitmap1 = new BitmapImage();
        //            bitmap1.UriSource = new Uri(Path1);
        //            BitmapImage bitmap2 = new BitmapImage();
        //            bitmap2.UriSource = new Uri(Path2);
        //            BitmapImage bitmap3 = new BitmapImage();
        //            bitmap3.UriSource = new Uri(Path3);
        //            img1.Source = bitmap1;
        //            img2.Source = bitmap2;
        //            img3.Source = bitmap3;
        //            ImgButtonPanel.Children.Add(img1);
        //            ImgButtonPanel.Children.Add(img2);
        //            ImgButtonPanel.Children.Add(img3);

        //        }


        //    }





        //}
        //ScenResponse data;
        //GoodSceneResponse data2;
        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    FittingRoomClient client = new FittingRoomClient();
        //    Button.Content = "Waiting";
        //    data = await client.GetScenesAsyc();
        //    Button.Content = data.response;



        //    for (int i = 0; i < data.scenslist.Count; i++)
        //    {
        //        var scene = data.scenslist.ElementAt(i);
        //        var key = scene.Key;
        //        var value = scene.Value;
        //        Button btn = new Button();
        //        btn.Width = 110;
        //        btn.Height = 110;
        //        btn.Content = value.enscene+ "\r\n" + value.flag;
        //        btn.Name = "Button" + i;
        //        scenButtonPanel.Children.Add(btn);
        //        btn.Click += ScenButton_Click;

        //    }


        //}


        //private async void ButtonTwo_Click(object sender, RoutedEventArgs e)
        //{
        //    FittingRoomClient client = new FittingRoomClient();
        //    data2 = await client.GetGoodsScenesAsyc();
        //    Button.Content = data2.response;
        //}


    }
}
