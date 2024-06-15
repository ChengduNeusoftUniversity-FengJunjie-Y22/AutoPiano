using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput.Native;

namespace AutoPiano
{
    /// <summary>
    /// PopupX.xaml 的交互逻辑
    /// </summary>
    public partial class PopupX : UserControl
    {
        public PopupX()
        {
            InitializeComponent();
            KEYA.Key = VirtualKeyCode.VK_A;
            KEYS.Key = VirtualKeyCode.VK_S;
            KEYD.Key = VirtualKeyCode.VK_D;
            KEYF.Key = VirtualKeyCode.VK_F;
            KEYG.Key = VirtualKeyCode.VK_G;
            KEYH.Key = VirtualKeyCode.VK_H;
            KEYJ.Key = VirtualKeyCode.VK_J;

            KEYZ.Key = VirtualKeyCode.VK_Z;
            KEYX.Key = VirtualKeyCode.VK_X;
            KEYC.Key = VirtualKeyCode.VK_C;
            KEYV.Key = VirtualKeyCode.VK_V;
            KEYB.Key = VirtualKeyCode.VK_B;
            KEYN.Key = VirtualKeyCode.VK_N;
            KEYM.Key = VirtualKeyCode.VK_M;

            KEYQ.Key = VirtualKeyCode.VK_Q;
            KEYW.Key = VirtualKeyCode.VK_W;
            KEYE.Key = VirtualKeyCode.VK_E;
            KEYR.Key = VirtualKeyCode.VK_R;
            KEYT.Key = VirtualKeyCode.VK_T;
            KEYY.Key = VirtualKeyCode.VK_Y;
            KEYU.Key = VirtualKeyCode.VK_U;
        }
        public RoutedEventHandler PopupClose
        {
            set
            {
                ClosePopup.SetButtonClick(value);
            }
        }
        public RoutedEventHandler SongSelect
        {
            set
            {
                SelectButton.SetButtonClick(value);
            }
        }
        public void UpdateProgress(double progress)
        {
            PShow.SetValue(progress);
        }
        public void UpdateAnimation(VirtualKeyCode target, int time)
        {
            KEYQ.FillAnimation(target, time);
            KEYW.FillAnimation(target, time);
            KEYE.FillAnimation(target, time);
            KEYR.FillAnimation(target, time);
            KEYT.FillAnimation(target, time);
            KEYY.FillAnimation(target, time);
            KEYU.FillAnimation(target, time);

            KEYA.FillAnimation(target, time);
            KEYS.FillAnimation(target, time);
            KEYD.FillAnimation(target, time);
            KEYF.FillAnimation(target, time);
            KEYG.FillAnimation(target, time);
            KEYH.FillAnimation(target, time);
            KEYJ.FillAnimation(target, time);

            KEYZ.FillAnimation(target, time);
            KEYX.FillAnimation(target, time);
            KEYC.FillAnimation(target, time);
            KEYV.FillAnimation(target, time);
            KEYB.FillAnimation(target, time);
            KEYN.FillAnimation(target, time);
            KEYM.FillAnimation(target, time);
        }
    }
}
