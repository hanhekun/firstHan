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
        private bool isPlaying;
        private DateTime mStartTime = DateTime.Now;
        public MainPage()
        {
            this.InitializeComponent();          
            listening = true;
            CreateServerAsync();
            
            mTimer.Tick += MTimer_Tick;
            mTimer.Interval = TimeSpan.FromSeconds(5);
           
        }

        private void createImg1()
        {
            Image img1 = new Image();
            BitmapImage bitmap = new BitmapImage();           
            bitmap.UriSource = new Uri(scenBitmaps[0]);
            img1.Source = bitmap;
            this.scenePanel.Children.Add(img1);
        }

        private void MStoryboard_Completed(object sender, object e)
        {
            isPlaying = false;
        }

        private void MTimer_Tick(object sender, object e)
        {
            if (isPlaying || DateTime.Now.Subtract(mStartTime).TotalSeconds < 5)
                return;
            play++;
            if (play== scenBitmaps.Count)
            {
                play = 0;
            }
            Image sceneImage2 = start(play);
            Storyboard story= FadeInEffect(scenePanel.Children[0], sceneImage2);
            isPlaying = true;
            story.Completed += Story_Completed;
            mStartTime = DateTime.Now;
            story.Begin();
        }

        private void Story_Completed(object sender, object e)
        {
            if (scenBitmaps.Count == 0)
                return;
            isPlaying = false;
            scenePanel.Children.RemoveAt(0);

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
                    createImg1();

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


        private async Task appearAsync(string request)
        {
            string[] data = request.Split(new char[] { ',' });
            string action = data[0];
            if (action == "scene")
            {
                if (isPlaying)
                {
                    await Task.Delay(1500);
                }
                int sceneId = int.Parse(data[1]);
                int scenIndex = int.Parse(data[2]);
                BitmapImage bitmap = new BitmapImage();

                int index = (sceneId - 1) * 3 + scenIndex - 1;
                bitmap.UriSource = new Uri(scenBitmaps[index]);
                if (index <= scenBitmaps.Count)
                {
                    Image sceneImage2 = start(index);
                    Storyboard story = FadeInEffect(scenePanel.Children[0], sceneImage2);
                    isPlaying = true;
                    story.Completed += Story_Completed;
                    mStartTime = DateTime.Now;
                    story.Begin();
                    mTimer.Start();
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
                    appearAsync(request);
                }));
            }
        }
        public static Storyboard FadeInEffect(UIElement pre, UIElement cur)
        {
            
            Storyboard story = new Storyboard();
            DoubleAnimation daIn = new DoubleAnimation();
            daIn.Duration = TimeSpan.FromMilliseconds(1500);
            daIn.From = 0;
            daIn.To = 1;
            DoubleAnimation daOut = new DoubleAnimation();
            daOut.From = 1;
            daOut.To = 0;
            daOut.Duration = TimeSpan.FromMilliseconds(1500);
            story.Children.Add(daIn);
            story.Children.Add(daOut);
           Storyboard.SetTarget(daIn, cur);
            Storyboard.SetTarget(daOut, pre);
            Storyboard.SetTargetProperty(daIn, "Opacity");
            Storyboard.SetTargetProperty(daOut, "Opacity");
            
            return story;
        }
        private Image start(int play)
        {
            Image img2 = new Image();
            BitmapImage bitmap = new BitmapImage();
            if (play+1 == scenBitmaps.Count)
            {
                play = -1;
            }
            bitmap.UriSource = new Uri(scenBitmaps[play+1]);
            img2.Source = bitmap;
            img2.Opacity = 0;
            this.scenePanel.Children.Add(img2);
            return img2;
            
        }
        

    }

}
