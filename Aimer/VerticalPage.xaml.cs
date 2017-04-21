using Aimer.SDK;
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
        private Dictionary<string, Scene> mAllScens;
        
        
        public VerticalPage()
        {            
            this.InitializeComponent();
            FillButtonAsync();
            vetrtcalPage.DOMContentLoaded += VetrtcalPage_DOMContentLoaded;
            vetrtcalPage.NewWindowRequested += WebView_NewWindowRequested;
            vetrtcalPage.AllowFocusWhenDisabled = false;
            vetrtcalPage.AllowFocusOnInteraction = false;
            
            vetrtcalPage.GotFocus += VetrtcalPage_GotFocus;
            vetrtcalPage.LostFocus += VetrtcalPage_LostFocus;

        }

        private void WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            sender.Navigate(args.Uri);
            args.Handled = true;
        }

        private void VetrtcalPage_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void VetrtcalPage_GotFocus(object sender, RoutedEventArgs e)
        {
            barCode.Focus(FocusState.Keyboard);

        }

        private void VetrtcalPage_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            barCode.Focus(FocusState.Keyboard);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
            GoodSceneResponse selectScenes =await  fittingRoom.GetGoodsScenesAsyc(str);
            if(selectScenes.response == "getgoodsscene")
            {
                string flag = selectScenes.flag;
                
                CutButton(flag);
            }
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
                Uri uri = new Uri("http://www.aimer.com.cn/goods/"+str);
                vetrtcalPage.Source = (uri);
                hideGoodsScene();
                getGoodsScenes(str);
                str = "";
            }

            else if (code >= 48 && code <= 57) //0-9
            {
               str = str + (args.KeyCode - 48);
            }
            else if (code >= 97 && code <= 122) //a-z
            {
                str = str + (char)code;
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
                    mAllScens = result.scenslist;
                    foreach (var sene in mAllScens)
                    {

                        ScenButton scenButton = new ScenButton();
                        scenButton.Click += ScenButton_Click;
                        scenButton.SetData(sene.Key ,sene.Value);
                        themeButtonPanel.Children.Add(scenButton);
                    }
                }
                
            }
            catch(Exception e)
            {
                Log.Error("get scens error",e);
            }
        }

        private void CutButton(string flag)
        {
            foreach(ScenButton i in themeButtonPanel.Children)
            {
                if (flag != i.getText())
                {
                    i.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ScenButton_Click(object sender, EventArgs e)
        {
            SceneButtonEventArgs args = e as SceneButtonEventArgs;
            Scene selectedScene = mAllScens[args.SceneId];
            secondSceneControl.SetData(selectedScene, args.SceneId);
            secondSceneControl.Show();
        }
    }


}
