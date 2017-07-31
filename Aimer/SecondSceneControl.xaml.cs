﻿using Aimer.SDK;
using MetroLog;
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
        private ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<SecondSceneControl>();
        private string mSceneId;        

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

        public void SetData(List<Scene> list, Scene scene, string sceneId)
        {

            RemoveListener();
            mSceneId = sceneId; 
            scenePanel.Children.Clear();
            Thickness margin = new Thickness(10, 40,10,0);

            List<Uri> uri = new List<Uri>(); 
            foreach (var myscene in list)
            {
                SceneItem item1 = new SceneItem();

                item1.SetData(myscene, 1);
                item1.Margin = margin;
                SceneItem item2 = new SceneItem();
                item2.SetData(myscene, 2);
                item2.Margin = margin;

                SceneItem item3 = new SceneItem();
                item3.SetData(myscene, 3);
                item3.Margin = margin;


                scenePanel.Children.Add(item1);
                scenePanel.Children.Add(item2);
                scenePanel.Children.Add(item3);

            }            
            scrollViewer.ScrollToHorizontalOffset(612*(int.Parse( sceneId)-1));

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
                var writer = MainPage.Writer;
                try
                {
                      await writer.WriteLineAsync($"scene,{mSceneId},{index}");
                     // await writer.WriteLineAsync($"{index}");

                    await writer.FlushAsync();
                }
                catch(Exception ex)
                {
                    Log.Error("Send command error", ex);
                }

            }
        }

        
    }
}
