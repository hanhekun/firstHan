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
            vetrtcalPage.NewWindowRequested += WebView_NewWindowRequested;
            
        }


        private void WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            sender.Navigate(args.Uri);
            barCode.Focus(FocusState.Pointer);
            args.Handled = true;
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
            try
            {
                FittingRoomClient fittingRoom = new FittingRoomClient();

                GoodSceneResponse selectScenes = await fittingRoom.GetGoodsScenesAsyc(str);
                if (selectScenes.response == "getgoodsscene")
                {
                    string flag = selectScenes.flag;

                    CutButton(flag);
                }
            }
            catch (Exception e)
            {

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
            Log.Debug(code.ToString());
            
            if (code == 13) //enter
            {
                Frame rootFrame = Window.Current.Content as Frame;
                MainPage main = rootFrame.Content as MainPage;
                string s = main.getText();
                if ( s!= "")
                {
                    str =s;
                }
                if(str != "")
                {
                    Uri uri = new Uri("http://www.aimer.com.cn/goods/" + str);
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
            else if (code >= 97 && code <= 122) //a-z
            {
                str = str + (char)code;
            }

        }

        public void UnFocus()
        {
            vetrtcalPage.Focus(FocusState.Unfocused);
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
        public void hideImg()
        {
            sceneSelectImg.Visibility = Visibility.Collapsed;
            buttonGrid.Visibility = Visibility.Collapsed;
        }
        public void AppearImg()
        {
            sceneSelectImg.Visibility = Visibility.Visible;
            buttonGrid.Visibility = Visibility.Visible;
        }
    }


}
