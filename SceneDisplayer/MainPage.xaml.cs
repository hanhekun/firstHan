using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SceneDisplayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string request;
        private bool listening;

        public MainPage()
        {
            this.InitializeComponent();
            listening = true;
            this.CreateServerAsync();
        }

        private void appear()
        {

            switch (request)
            {
                case "red":
                    red.Visibility = Visibility.Visible;
                    break;
                case "blue":
                    blue.Visibility = Visibility.Visible;
                    break;
                case "yellow":
                    yellow.Visibility = Visibility.Visible;
                    break;

            }
        }

        private async System.Threading.Tasks.Task CreateServerAsync()
        {
            try
            {
                //Create a StreamSocketListener to start listening for TCP connections.
                Windows.Networking.Sockets.StreamSocketListener socketListener = new Windows.Networking.Sockets.StreamSocketListener();

                //Hook up an event handler to call when connections are received.
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;

                //Start listening for incoming TCP connections on the specified port. You can specify any port that' s not currently in use.
                await socketListener.BindServiceNameAsync("1337");
            }
            catch (Exception e)
            {
                //Handle exception.
            }
        }
        

        private async void SocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender,
    Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            //Read line from the remote client.
            Stream inStream = args.Socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(inStream);
            while(listening)
            {
                request = await reader.ReadLineAsync();

                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, appear);
            }
        }

    

    }

}
