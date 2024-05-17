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

namespace AutoPiano
{
    /// <summary>
    /// Sidebar.xaml 的交互逻辑
    /// </summary>
    public partial class Sidebar : Border
    {
        public static Sidebar? Instance;
        public Sidebar()
        {
            InitializeComponent();
            Instance = this;
        }

        public void ExpandTAB()
        {
            // 创建一个双精度动画对象，用于改变按钮的宽度
            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.AccelerationRatio = 1;
            widthAnimation.From = 0; // 起始宽度
            widthAnimation.To = 240;   // 结束宽度
            widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2)); // 持续时间

            // 创建一个故事板，并将动画对象添加到其中
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);

            // 将动画应用到按钮的宽度属性
            Storyboard.SetTarget(widthAnimation, this);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

            // 启动动画
            storyboard.Begin();
        }

        public void UnExpandTAB()
        {
            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.AccelerationRatio = 1;
            widthAnimation.From = 240; // 起始宽度
            widthAnimation.To = 0;   // 结束宽度
            widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2)); // 持续时间

            // 创建一个故事板，并将动画对象添加到其中
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);

            // 将动画应用到按钮的宽度属性
            Storyboard.SetTarget(widthAnimation, this);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

            // 启动动画
            storyboard.Begin();
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            UnExpandTAB();
        }
    }
}
