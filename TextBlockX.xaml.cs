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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput.Native;

namespace AutoPiano
{
    /// <summary>
    /// TextBlockX.xaml 的交互逻辑
    /// </summary>
    public partial class TextBlockX : UserControl
    {
        public TextBlockX()
        {
            InitializeComponent();
        }

        public string TextX
        {
            set => Txt.Text = value;
        }

        public double FontSizeX
        {
            set => Txt.FontSize = value;
        }

        public SolidColorBrush ForegroundX
        {
            set => Txt.Foreground = value;
        }

        /// <summary>
        /// 控件对应的Key
        /// </summary>
        public VirtualKeyCode Key = VirtualKeyCode.VK_P;

        /// <summary>
        /// 动画执行器
        /// </summary>
        /// <param name="target">需要执行动画的按键码</param>
        /// <param name="time">填充耗费时长</param>
        /// <returns></returns>
        public void FillAnimation(VirtualKeyCode target, int time)
        {
            if (target != Key) { return; }
            DoubleAnimation ColorFillAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds((double)time / 1000),
                AccelerationRatio = 1
            };

            FillGrid.BeginAnimation(OpacityProperty, ColorFillAnimation);
        }
    }
}
