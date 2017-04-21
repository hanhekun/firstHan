using Aimer.SDK;
using MetroLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SceneDisplayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        private bool listening;
        private Dictionary<string, Scene> mAllScens;
        private DispatcherTimer mTimer = new DispatcherTimer();
        private int play=0;
        private IList<string> scenBitmaps = new List<string>();
        Storyboard mStoryboard;
        private bool isPlaying;
        public MainPage()
        {
            this.InitializeComponent();
            mStoryboard = FindName("FadeIn") as Storyboard;
             mStoryboard.FillBehavior = FillBehavior.Stop;
            mStoryboard.Completed += MStoryboard_Completed;
            listening = true;
            CreateServerAsync();
            mTimer.Tick += MTimer_Tick;
            mTimer.Interval = TimeSpan.FromSeconds(10);
        }

        private void MStoryboard_Completed(object sender, object e)
        {
            isPlaying = false;
        }

        private void MTimer_Tick(object sender, object e)
        {
            if (isPlaying)
                return;
            play++;
            if (play== scenBitmaps.Count)
            {
                play = 0;
            }
            sceneImage2.Opacity = 0;
            mStoryboard.Begin();
            isPlaying = true;
            Log.Debug(play+"");

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GetAllScenes();
          
        }

        private async void GetAllScenes()
        {
            FittingRoomClient room = new FittingRoomClient();
            try
            {
                var result = await room.GetScenesAsyc();
                if (result.response == "getscenes")
                {
                    mAllScens = result.scenslist;
                    for (var i = 0; i < mAllScens.Count; i++)
                    {
                        var Path1 = mAllScens.ElementAt(i).Value.imgpath1;
                        var Path2 = mAllScens.ElementAt(i).Value.imgpath2;
                        var Path3 = mAllScens.ElementAt(i).Value.imgpath3;
                        Path1 = await  LoadBitmap(Path1, mAllScens.ElementAt(i).Key, "1");
                        scenBitmaps.Add(Path1);
                        Path2 = await LoadBitmap(Path2, mAllScens.ElementAt(i).Key, "2");
                        scenBitmaps.Add(Path2);
                        Path3 = await LoadBitmap(Path3, mAllScens.ElementAt(i).Key, "3");
                        scenBitmaps.Add(Path3);
                    }                    
                }
                mTimer.Start();
            }
            catch (Exception e)
            {
                Log.Error("get scens error", e);
            }
        }

        private async Task<string> LoadBitmap(string path, string sceneId, string sceneIndex)
        {
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Scen", CreationCollisionOption.OpenIfExists);
                var filePath = folder.Path + "/" + sceneId + "_" + sceneIndex + ".jpg";
                if (File.Exists(filePath))
                    return filePath;

                var tmp = filePath + ".download";
                using (HttpClient http = new HttpClient())
                using (FileStream fs = File.OpenWrite(tmp))
                {
                    var stream = await http.GetStreamAsync(path);
                    await stream.CopyToAsync(fs);
                }
                File.Move(tmp, filePath);
                return filePath;
            }
            catch(Exception e)
            {
                Log.Error("Download Image Error", e);
            }
            
            return null;
        }


        private void appear(string request)
        {
            string[] data = request.Split(new char[] { ',' });
            string action = data[0];
            if (action == "scene")
            {
                int sceneId=int.Parse(data[1]);
                int scenIndex = int.Parse(data[2]);
                BitmapImage bitmap = new BitmapImage();
                
                int index = (sceneId-1) * 3 + scenIndex-1;
                bitmap.UriSource = new Uri(scenBitmaps[index]);
                if (index<= scenBitmaps.Count)
                {
                    sceneImage.Source = bitmap;
                }
                
            }
        }
        Windows.Networking.Sockets.StreamSocketListener socketListener;
        private async System.Threading.Tasks.Task CreateServerAsync()
        {
            try
            {
                socketListener = new Windows.Networking.Sockets.StreamSocketListener();
                socketListener.ConnectionReceived += SocketListener_ConnectionReceived;
                
                await socketListener.BindServiceNameAsync("1337");
            }
            catch (Exception e)
            {
                Log.Error("Create Server On Port 1337 Error", e);
            }
        }
        

        private async void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            //Read line from the remote client.
            Stream inStream = args.Socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(inStream);
            while(listening)
            {
                string request = await reader.ReadLineAsync();
                if (request == null)
                    break;
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    appear(request);
                }));
            }
        }

    

    }

}
