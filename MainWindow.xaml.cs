using System.Text;
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
using System.Windows.Threading;

namespace AutoPiano
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            StartMouseTrailAnimation();
        }

        private void StartMouseTrailAnimation()
        {
            // 创建一个双精度动画对象，用于改变拖尾圆的透明度
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 1.0; // 起始透明度
            opacityAnimation.To = 0.0;   // 结束透明度
            opacityAnimation.Duration = TimeSpan.FromSeconds(0.5); // 持续时间
            opacityAnimation.AutoReverse = true; // 自动反向播放

            // 创建一个故事板，并将动画对象添加到其中
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);

            // 将动画应用到拖尾圆的透明度属性
            Storyboard.SetTarget(opacityAnimation, Trail);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            // 监听鼠标移动事件
            MouseMove += (sender, e) =>
            {
                // 获取鼠标当前位置
                Point currentPosition = e.GetPosition(this);

                // 设置拖尾圆的位置
                Canvas.SetLeft(Trail, currentPosition.X);
                Canvas.SetTop(Trail, currentPosition.Y);

                // 启动动画
                storyboard.Begin();
            };
        }

        private void Selects_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabA.ExpandTAB();
            e.Handled = true;
        }

        private void Selects_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabA.UnExpandTAB();
            e.Handled = true;
        }
    }
}