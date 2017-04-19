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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Aimer
{
    public sealed partial class ScenButton : UserControl
    {
        Dictionary<string, string> images = new Dictionary<string, string>();
        private string mSceneId;
        private Storyboard pointerDown;
        private Storyboard pointerUp;

        public event EventHandler Click;

        public ScenButton() 
        {
            this.InitializeComponent();

            images.Add("home", "Assets/Images/床.png");
            images.Add("vacation", "Assets/Images/飞机.png");
            images.Add("lovely", "Assets/Images/爱心.png");
            images.Add("night", "Assets/Images/嘴唇.png");
            images.Add("outdoor", "Assets/Images/指南针.png");
            images.Add("men", "Assets/Images/男士.png");

            pointerDown = FindName("PointDownStoryboard") as Storyboard;
            pointerUp = FindName("PointUpStoryboard") as Storyboard;
            pointerUp.Completed += PointerUp_Completed;
        }

        private void PointerUp_Completed(object sender, object e)
        {
            RaiseClientEvent();
            
        }

        public void SetData(string scenId, Scene mScene)
        {
            mSceneId = scenId;
            seneNameText.Text = mScene.flag;
            seneNameEnText.Text = mScene.enscene;
            if(images.ContainsKey(mScene.enscene))
            {
                var imagePath ="ms-appx:///" + images[mScene.enscene];
                BitmapImage bitmap = new BitmapImage();
                bitmap.UriSource = new Uri( imagePath);
                int width = bitmap.PixelHeight;
                image.Source = bitmap;
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            pointerDown.Begin();
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            pointerUp.Begin();
        }

        private void RaiseClientEvent()
        {
            if(Click != null)
            {
                Click(this, new SceneButtonEventArgs(mSceneId));
            }
        }
    }
}
