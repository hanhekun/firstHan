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

        public VerticalPage()
        {
            this.InitializeComponent();
        }

        private void CoreWindow_CharacterReceived(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.CharacterReceivedEventArgs args)
        {
            var code = args.KeyCode;
            if (code == 13) //enter
            {
                //NavigateToDetail();
            }
            else if (code == 8) //backspace
            {
                //DeleteCharacter();
            }
            else if (code >= 48 && code <= 57) //0-9
            {
               // txt.Text = txt.Text + (args.KeyCode - 48);
            }
            else if (code >= 97 && code <= 122) //a-z
            {
               // txt.Text = txt.Text + (char)code;
            }
        }

      
    }


}
