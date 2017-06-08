using Aimer.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace test1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            AA();
        }

        private async void AA()
        {
            FittingRoomClient room = new FittingRoomClient();
            var result = await room.GetScenesAsyc();
            MediaPlayerElement video = new MediaPlayerElement();
            Uri uri=new Uri( result.scenslist.ElementAt(0).scene3.imgpath);
            video.Source = MediaSource.CreateFromUri(uri);
            video.AutoPlay = true;
            grid.Children.Add(video);
        }
    }
}
