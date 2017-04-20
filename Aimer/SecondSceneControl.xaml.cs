using Aimer.SDK;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Aimer
{
    public sealed partial class SecondSceneControl : UserControl
    {
        public SecondSceneControl()
        {
            this.InitializeComponent();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void SetData(Scene scene)
        {
            RemoveListener();
            scenePanel.Children.Clear();
            Thickness margin = new Thickness(0, 0, 20, 0);
            SceneItem item1 = new SceneItem();            
            item1.Margin = margin;
            item1.SetData(scene, 1);

            SceneItem item2 = new SceneItem();            
            item2.SetData(scene, 2);
            item2.Margin = margin;
            SceneItem item3 = new SceneItem();
            item3.SetData(scene, 3);
            scenePanel.Children.Add(item1);
            scenePanel.Children.Add(item2);
            scenePanel.Children.Add(item3);
            AddListener();


        }

        private void AddListener()
        {
            IEnumerable<SceneItem> items = scenePanel.Children.OfType<SceneItem>();
            foreach (var item in items)
            {
                item.Click += Item_ClickAsync;
            }
        }

        private void RemoveListener()
        {
            IEnumerable<SceneItem> items = scenePanel.Children.OfType<SceneItem>();
            foreach (var item in items)
            {
                item.Click -= Item_ClickAsync;
            }
        }

        private async void Item_ClickAsync(object sender, EventArgs e)
        {
            SceneSelectEventArgs args = e as SceneSelectEventArgs;
            var index = args.Index;
            IEnumerable<SceneItem> items = scenePanel.Children.OfType<SceneItem>();
            foreach (var item in items)
            {
                item.Select(index);
                Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut);

                await writer.WriteLineAsync("qqq");
                await writer.FlushAsync();
            }
        }
    }
}
