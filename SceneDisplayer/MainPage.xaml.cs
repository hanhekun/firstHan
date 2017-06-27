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
using Windows.Media.Core;
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
        private Boolean isVideo=false;
        private ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        private bool listening;
        private Dictionary<string, Scene> mAllScens;
        private DispatcherTimer mTimer = new DispatcherTimer();
        private DispatcherTimer defaultTimer = new DispatcherTimer();
        private Storyboard story;

        private Boolean isFirst=true;
        private int play=0;
        private IList<ScenePath> scenBitmaps = new List<ScenePath>();
        private bool isPlaying=false;
        private DateTime mStartTime = DateTime.Now;
        private Boolean isConnect = false;
        private string scene;
        public MainPage()
        {
            this.InitializeComponent();          
            listening = true;
            CreateServerAsync();
            
            mTimer.Tick += MTimer_Tick;
            mTimer.Interval = TimeSpan.FromSeconds(10);
            defaultTimer.Tick += DefaultTimer_Tick;
            defaultTimer.Interval = TimeSpan.FromSeconds(600);
            GetAllScenes();

        }

        private void DefaultTimer_Tick(object sender, object e)
        {
            isFirst = false;
            GetAllScenes();
        }

        private void createImg1()
        {
            Image img1 = new Image();
            BitmapImage bitmap = new BitmapImage();
            if (bitmap.UriSource == null)
            {
                bitmap.UriSource = new Uri("ms-appx:///Assets/loadfailed.jpg");
            }
            bitmap.UriSource = new Uri(scenBitmaps[0].imgpath);
            img1.Source = bitmap;
            img1.Stretch = Stretch.Fill;
            this.scenePanel.Children.Add(img1);
        }

        private void MStoryboard_Completed(object sender, object e)
        {
            isPlaying = false;
        }

        private void MTimer_Tick(object sender, object e)
        {

            //if(scenePanel.Children[0] is MediaElement)
            //{
            //    MediaElement video = scenePanel.Children[0] as MediaElement;

            //}
            this.scenePanel.Children.Clear();
            play++;
            if (play== scenBitmaps.Count)
            {
                play = 0;
            }
            
            if (scenBitmaps.Count > 0)
            {
                if (scenBitmaps[play].type == "图片")
                {
                    Image sceneImage2 = start(play);
                    //story = FadeInEffect(scenePanel.Children[0], sceneImage2);
                }
                else
                {
                    MediaElement video2 = startVideo(play);
                    //story = FadeInEffect(scenePanel.Children[0], video2);
                }
                isPlaying = true;
                //story.Completed += Story_Completed;
                //mStartTime = DateTime.Now;
                //story.Begin();
            }
            
        }

        private void Story_Completed(object sender, object e)
        {
            if (scenBitmaps.Count == 0)
                return;
            isPlaying = false;
            scenePanel.Children.RemoveAt(0);

        }

        private async void GetAllScenes()
        {
            if (isFirst == false)
            {
                scenBitmaps.Clear();
            }
            FittingRoomClient room = new FittingRoomClient();
            try
            {
                var result = await room.GetScenesAsyc();
                
                if (result.response == "getscenes")
                {
                    var img1Type = result.scenslist.ElementAt(6).scene1.type;
                    var path1 = result.scenslist.ElementAt(6).scene1;
                    var path2 = result.scenslist.ElementAt(6).scene2;
                    var path3 = result.scenslist.ElementAt(6).scene3;
                    
                    if (img1Type == "图片")
                    {
                        path1.imgpath = await LoadBitmap(path1.imgpath.ToString(),"jpg");
                        scenBitmaps.Add(path1);
                        createImg1();
                    }
                    if (img1Type == "视频")
                    {
                        scenBitmaps.Add(path1);
                        MediaElement video = new MediaElement();
                        Uri uri = new Uri(result.scenslist.ElementAt(0).scene1.imgpath);
                        video.Source = uri;
                        video.Stretch = Stretch.Fill;
                        video.AutoPlay = true;
                        scenePanel.Children.Add(video);
                    }
                    scenBitmaps.Add(path2);
                    scenBitmaps.Add(path3);
                }
                mTimer.Start();
            }
            catch (Exception e)
            {
                internetSourceFailed.Visibility = Visibility.Visible;
                Log.Error("get scens error", e);
            }
        }
        private async void ChangeScenes(string scene)
        {
            scenBitmaps.Clear();
            FittingRoomClient room = new FittingRoomClient();
            
                play = 0;            
            try
            {
                var result = await room.GetScenesAsyc();

                if (result.response == "getscenes")
                {
                    List<ScenePath> scenepath = new List<ScenePath>();
                    
                    foreach (var i in result.scenslist)
                    {
                        
                        if (i.flag== scene)
                        {
                            var img1Type = i.scene1.type;

                            var path1 = i.scene1;
                            var path2 = i.scene2;
                            var path3 = i.scene3;
                            
                            if (img1Type == "图片")
                            {
                                path1.imgpath = await LoadBitmap(path1.imgpath.ToString(),"jpg");
                                scenBitmaps.Add(path1);
                                createImg1();
                            }
                            else
                            {
                                MediaElement video = new MediaElement();
                                Uri uri = new Uri(i.scene1.imgpath);
                                video.Source = uri;
                                video.Stretch = Stretch.Fill;
                                video.AutoPlay = true;
                                scenePanel.Children.Add(video);
                                scenBitmaps.Add(path1);
                            }
                            if (i.scene2.type == "图片")
                            {
                                path2.imgpath = await LoadBitmap(path2.imgpath.ToString(), "jpg");
                            }
                            else
                            {
                                path2.imgpath = await LoadBitmap(path2.imgpath.ToString(),"mp4");
                            }
                            if (i.scene3.type == "图片")
                            {
                                path3.imgpath = await LoadBitmap(path3.imgpath.ToString(), "jpg");
                            }
                            else
                            {
                                path3.imgpath = await LoadBitmap(path3.imgpath.ToString(), "mp4");
                            }

                            scenBitmaps.Add(path2);
                            scenBitmaps.Add(path3);
                        }   
                    }
                    createImg1();

                }
                mTimer.Start();
            }
            catch (Exception e)
            {
                internetSourceFailed.Visibility = Visibility.Visible;
                Log.Error("get scens error", e);
            }
        }

        private async Task<string> LoadBitmap(string path,string type)
        {
            return path;
            //try
            //{
            //    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Scen", CreationCollisionOption.OpenIfExists);
            //    string str = folder.Path.Replace(":", "").Replace(".", "").Replace("?", "").Replace("\\", "").Replace("-", "");
            //    var filePath = folder.Path + str.Substring(str.Length - 5) + "." + type;
            //    if (File.Exists(filePath))
            //        return filePath;

            //    var tmp = folder.Path + ".download";
            //    using (HttpClient http = new HttpClient())
            //    using (FileStream fs = File.OpenWrite(tmp))
            //    {
            //        var stream = await http.GetStreamAsync(path);
            //        await stream.CopyToAsync(fs);
            //    }
            //    File.Move(tmp, filePath);
            //    return filePath;
            //}
            //catch (Exception e)
            //{
            //    return path;
            //}
        }


        private async Task appearAsync(string request)
        {

            string[] data = request.Split(new char[] { ',' });
            string action = data[0];
            if (action == "scene")
            {                

                if (isPlaying)
                {
                    await Task.Delay(1000);
                }
                int scenIndex = int.Parse(data[2]);
                BitmapImage bitmap = new BitmapImage();

                int index = scenIndex;
                
                bitmap.UriSource = new Uri(scenBitmaps[index-1].imgpath);
                this.scenePanel.Children.Clear();

                if (index <= scenBitmaps.Count)
                {

                    
                    if (scenBitmaps[index - 1].type == "图片")
                    {
                        Image sceneImage2 = start(index - 1);
                        //this.scenePanel.Children.Add(sceneImage2);
                        //story = FadeInEffect(scenePanel.Children[0], sceneImage2);
                    }
                    else
                    {
                        MediaElement video2 = startVideo(index - 1);
                        //this.scenePanel.Children.Add(video2);
                        //story = FadeInEffect(scenePanel.Children[0], video2);

                    }
                    isPlaying = true;
                    //story.Completed += Story_Completed;
                    //mStartTime = DateTime.Now;
                    //story.Begin();
                    //mTimer.Start();
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
            isConnect = true;
            Stream inStream = args.Socket.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(inStream);
            if (isConnect)
            {
                Stream outStream = args.Socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(outStream);
                await writer.WriteLineAsync("已经有设备连接，禁止连接");
                //await writer.FlushAsync();
            }              
            while (listening)
            {
                string request;
                try
                {
                    request = await reader.ReadLineAsync();
                }
                catch (Exception e)
                {
                    request= null;
                }
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        notConnect.Visibility = Visibility.Collapsed;
                        disposeConnect.Visibility = Visibility.Collapsed;
                        loadFail.Visibility = Visibility.Collapsed;

                        switch (request)
                        {
                            case "家居":
                            case "泳衣":

                            case "可爱":
                            case "性感":
                            case "户外":
                            case "绅士":
                                scene = request;
                                defaultTimer.Start();
                                ChangeScenes(scene);
                                break;
                            case "testConnect":
                                break;
                        }
                        
                    }));
                    if (request == null)
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                        {
                            disposeConnect.Visibility = Visibility.Visible;
                            isConnect = false;
                            
                        }));
                    
                        break;
                }
                else
                {
                    
               
                              
                  
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                    
                        if (request.Contains("scene"))
                        {
                            if (isVideo == true)
                            {
                                foreach (var i in scenePanel.Children)
                                {
                                    if (i is MediaElement)
                                    {
                                        var video = i as MediaElement;
                                        video.Stop();
                                        mTimer.Start();
                                    }
                                }
                            }
                        }
                        appearAsync(request);

                    }));
                }
                
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
            isVideo = false;
            Image img2 = new Image();
            BitmapImage bitmap = new BitmapImage();
            if (play == scenBitmaps.Count)
            {
                play = 0;
            }
            
            bitmap.UriSource = new Uri(scenBitmaps[play].imgpath);
            if (bitmap.UriSource==null)
            {
                loadFail.Visibility = Visibility.Visible;
            }           
            img2.Source = bitmap;
            img2.Stretch = Stretch.Fill;
            this.scenePanel.Children.Add(img2);
            return img2;
            
        }
        private MediaElement startVideo(int play)
        {
            MediaElement video = new MediaElement();
            video.AutoPlay = true;
            if (play == scenBitmaps.Count)
            {
                play = 0;
            }
            isVideo = true;
            video.MediaEnded += Video_MediaEnded;
            Uri uri = new Uri(scenBitmaps[play].imgpath);
            video.Source = uri;
            video.Stretch = Stretch.Fill;
            if (uri== null)
            {
                loadFail.Visibility = Visibility.Visible;
            }
            this.scenePanel.Children.Add(video);
            return video;
        }

        private void Video_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement video = sender as MediaElement;
            video.Play();
        }
    }

}
