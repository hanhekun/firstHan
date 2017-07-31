using Aimer.SDK;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Aimer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VerticalPage : Page
    {
        private ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<VerticalPage>();
        private string str = "";
        //private Dictionary<string, Scene> mAllScens;
        private List<Scene> mAllScens=new List<Scene>();

        private DispatcherTimer timer = new DispatcherTimer();

        private const string ScanFormat = "http://m.aimer.com.cn/goods?sid={0}&u_id=20045&sub_id=a1gmsyj";
        //private const string ScanFormat = "http://m.aimer.com.cn/goods/";
        BarcodeScanner scanner = new BarcodeScanner();
        public VerticalPage()
        {            
            this.InitializeComponent();
            FillButtonAsync();
            vetrtcalPage.DOMContentLoaded += VetrtcalPage_DOMContentLoaded;
            vetrtcalPage.NewWindowRequested += WebView_NewWindowRequested;
            vetrtcalPage.NavigationCompleted += VetrtcalPage_NavigationCompleted;
            
            vetrtcalPage.GotFocus += VetrtcalPage_GotFocus;
            vetrtcalPage.LostFocus += VetrtcalPage_LostFocus;

            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);

            scanner.BarcodeScanned += Scanner_BarcodeScanned;
            Task.Factory.StartNew(async () =>
            {
                await scanner.StartAsync();
            });
        }

        private void Scanner_BarcodeScanned(object sender, BarcodeScannedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void VetrtcalPage_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
           // barCode.Focus(FocusState.Pointer);
        }

        private void Timer_Tick(object sender, object e)
        {
            barCode.Focus(FocusState.Pointer);
        }

        private void WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            //barCode.Focus(FocusState.Pointer);
            sender.Navigate(args.Uri);
            args.Handled = true;
        }

        private void VetrtcalPage_LostFocus(object sender, RoutedEventArgs e)
        {
           // barCode.Focus(FocusState.Programmatic);
        }

        private void VetrtcalPage_GotFocus(object sender, RoutedEventArgs e)
        {
            timer.Start();           
        }

        private void VetrtcalPage_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            //barCode.Focus(FocusState.Programmatic);
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //barCode.Focus(FocusState.Programmatic);
            base.OnNavigatedTo(e);
            Window.Current.CoreWindow.CharacterReceived += CoreWindow_CharacterReceived;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Window.Current.CoreWindow.CharacterReceived -= CoreWindow_CharacterReceived;
        }

        private async void getGoodsScenes(string str)
        {
            FittingRoomClient fittingRoom = new FittingRoomClient();
            GoodSceneResponse selectScenes = await fittingRoom.GetGoodsScenesAsyc(str);
            if (selectScenes.response == "getgoodsscene")
            {
                string flag = selectScenes.flag;
                SelectScene(flag);
            }
            //MediaPlayerElement a = new MediaPlayerElement();
            

        }

        private void FilterScene(int sceneId)
        {

        }

        private void hideGoodsScene()
        {
            secondSceneControl.Visibility = Visibility.Collapsed;
        }
        private void CoreWindow_CharacterReceived(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.CharacterReceivedEventArgs args)
        {

            var code = args.KeyCode;
            if (code == 13) //enter
            {
                Frame rootFrame = Window.Current.Content as Frame;
                MainPage main = rootFrame.Content as MainPage;
                main.DisLogo();
                string s = main.getText();
                if (s != "")
                {
                    str = s;
                }
                if (str != ""){
                    var targetProduct = string.Format(ScanFormat, str);
                    Uri uri = new Uri(targetProduct);
                     vetrtcalPage.Source = (uri);
                    hideGoodsScene();
                    getGoodsScenes(str);
                }
                
                str = "";
            }

            else if (code >= 48 && code <= 57) //0-9
            {
                str = str + (args.KeyCode - 48);
            }
            else if ((code >= 65 && code <= 90) || (code >= 97 && code <= 122)) //A-Z,a-z
            {
                str = str + (char)code;
            }
            else if (code == 45)
            {
                str += (char)code;
            }
        }

        private async void FillButtonAsync()
        {
            FittingRoomClient room = new FittingRoomClient();
            try
            {
                var result = await room.GetScenesAsyc();
                
                if(result.response == "getscenes")
                {
                    
                    foreach (var i in result.scenslist)
                    {
                        if (i.flag != "默认")
                        {
                            mAllScens.Add(i);
                            ScenButton scenButton = new ScenButton();
                            scenButton.Click += ScenButton_ClickAsync;
                            scenButton.SetData(i.flag, i);
                            themeButtonPanel.Children.Add(scenButton);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error("get scens error",e);
            }
        }

        private void SelectScene(string flag)
        {
            foreach (ScenButton i in themeButtonPanel.Children)
            {

                if (flag == i.getText())
                {
                    string id = i.GetSceneId ();
                    Scene sc = mAllScens[int.Parse(id)];
                    //i.Visibility = Visibility.Collapsed;
                    secondSceneControl.SetData(mAllScens,sc, id);
                    secondSceneControl.Show();
                    SendBtnAsync(id);
                }
            }
        }

        private async void ScenButton_ClickAsync(object sender, EventArgs e)
        {           
            SceneButtonEventArgs args = e as SceneButtonEventArgs;
            Scene selectedScene = mAllScens[int.Parse(args.SceneId)-1];
            secondSceneControl.SetData(mAllScens,selectedScene, args.SceneId);
            secondSceneControl.Show();

            var SB = sender as ScenButton;
            var id = SB.GetSceneId();

            SendBtnAsync(id);
        }
        private async void SendBtnAsync(string id)
        {
            string s = "";
            switch (id)
            {
                case "1":
                    s = "家居";
                    break;
                case "2":
                    s = "泳衣";
                    break;
                case "3":
                    s = "可爱";
                    break;
                case "4":
                    s = "性感";
                    break;
                case "5":
                    s = "户外";
                    break;
                case "6":
                    s = "绅士";
                    break;
            }

            Frame rootFrame = Window.Current.Content as Frame;
            MainPage main = rootFrame.Content as MainPage;
            if (s != "")
                await main.SenderSceneAsync(s);
        }

        public void hideImg()
        {
            sceneSelectImg.Visibility = Visibility.Collapsed;
            buttonGrid.Visibility = Visibility.Collapsed;
            secondSceneControl.Hide();
        }
        public void HideButton()
        {
            secondSceneControl.Visibility = Visibility.Collapsed;
            secondSceneControl.Hide();
        }
        public void AppearImg()
        {
            sceneSelectImg.Visibility = Visibility.Visible;
            buttonGrid.Visibility = Visibility.Visible;
        }

        private void barCode_Click(object sender, RoutedEventArgs e)
        {

        }
    }


}
