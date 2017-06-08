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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Aimer
{
    public sealed partial class SceneItem : UserControl
    {
        public event EventHandler Click;
        private int mIndex;
        //private string scene;
        public SceneItem()
        {
            this.InitializeComponent();
        }

        public void SetData(Scene scene, int index)
        {
            mIndex = index;
            BitmapImage bitmap = new BitmapImage();
            switch (index)
            {
                case 1:
                    sceneNameText.Text = "场景一";
                    bitmap.UriSource = new Uri(scene.scene1.thumbnail);
                    break;
                case 2:
                    sceneNameText.Text = "场景二";
                    bitmap.UriSource = new Uri(scene.scene2.thumbnail);
                    break;
                case 3:
                    sceneNameText.Text = "场景三";
                    bitmap.UriSource = new Uri(scene.scene3.thumbnail);
                    break;
            }
           sceneImage.Source = bitmap;            
        }

        public void Select(int index)
        {
            if(index != mIndex)
            {
                selectBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                selectBorder.Visibility = Visibility.Visible;
            }
        }

        private void RaiseClickEvent()
        {
            if (Click != null)
            
                Click(this, new SceneSelectEventArgs(mIndex));                  
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            RaiseClickEvent();
        }



    }
}
