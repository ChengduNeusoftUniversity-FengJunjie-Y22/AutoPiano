using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AutoPiano
{
    /// <summary>
    /// ButtonX.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonX : UserControl
    {
        public ButtonX()
        {
            InitializeComponent();
            BT.Click += (sender, e) =>
            {
                GridAnimation("BackGrid", OpacityProperty, 0.5, 0);
            };
        }

        /// <summary>
        /// 按钮提示词
        /// </summary>
        public string ButtonText
        {
            set => BT.Content = value;
        }

        /// <summary>
        /// 按钮提示词颜色
        /// </summary>
        public SolidColorBrush ButtonTextColor
        {
            set => BT.Foreground = value;
        }

        /// <summary>
        /// 按钮提示词大小
        /// </summary>
        public double ButtonTextSize
        {
            set => BT.FontSize = value;
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public RoutedEventHandler Click
        {
            set => BT.Click += value;
        }

        public SolidColorBrush BorderAnimationColor
        {
            set => BT.BorderBrush = value;
        }

        /// <summary>
        /// 决定哪些边框需要动画效果
        /// </summary>
        public Thickness BorderAnimationSide
        {
            set => BT.BorderThickness = value;
        }

        private double _bat = 0.3f;
        /// <summary>
        /// 边框动画持续时长
        /// </summary>
        public double BorderAnimationTime
        {
            get => _bat;
            set
            {
                if (value > 0) { _bat = value; }
            }
        }

        private double _gat = 0.3f;
        /// <summary>
        /// 填充背景动画持续时长
        /// </summary>
        public double GridAnimationTime
        {
            get => _gat;
            set
            {
                if (value > 0) { _gat = value; }
            }
        }

        private double _baa = 1f;
        /// <summary>
        /// 填充背景动画加速度
        /// </summary>
        public double BorderAnimationAccelerationRatio
        {
            get => _baa;
            set
            {
                if (value >= 0 && value <= 1) { _baa = value; }
            }
        }

        private double _gaa = 1f;
        /// <summary>
        /// 边框动画加速度
        /// </summary>
        public double GridAnimationAccelerationRatio
        {
            get => _gaa;
            set
            {
                if (value >= 0 && value <= 1) { _gaa = value; }
            }
        }

        private SolidColorBrush _hoverTextColor = Brushes.Cyan;
        /// <summary>
        /// 鼠标悬停时的字体色
        /// </summary>
        public SolidColorBrush HoverTextColor
        {
            get => _hoverTextColor;
            set => _hoverTextColor = value;
        }

        /// <summary>
        /// 鼠标进入前，提示词的颜色
        /// </summary>
        private Brush TempColor = Brushes.Transparent;

        private void WhileMouseEnter(object sender, MouseEventArgs e)
        {
            TempColor = BT.Foreground;
            BT.Foreground = HoverTextColor;
            if (BT.BorderThickness.Left > 0) BorderAnimation("LeftBorder", HeightProperty, 0, BT.ActualHeight);
            if (BT.BorderThickness.Top > 0) BorderAnimation("TopBorder", WidthProperty, 0, BT.ActualWidth);
            if (BT.BorderThickness.Right > 0) BorderAnimation("RightBorder", HeightProperty, 0, BT.ActualHeight);
            if (BT.BorderThickness.Bottom > 0) BorderAnimation("BottomBorder", WidthProperty, 0, BT.ActualWidth);
        }
        private void WhileMouseLeave(object sender, MouseEventArgs e)
        {
            BT.Foreground = TempColor;
            if (BT.BorderThickness.Left > 0) BorderAnimation("LeftBorder", HeightProperty, BT.ActualHeight, 0);
            if (BT.BorderThickness.Top > 0) BorderAnimation("TopBorder", WidthProperty, BT.ActualWidth, 0);
            if (BT.BorderThickness.Right > 0) BorderAnimation("RightBorder", HeightProperty, BT.ActualHeight, 0);
            if (BT.BorderThickness.Bottom > 0) BorderAnimation("BottomBorder", WidthProperty, BT.ActualWidth, 0);
        }
        private void BorderAnimation(string name, DependencyProperty dp, double start, double end)
        {
            Border target = BT.Template.FindName(name, BT) as Border;
            DoubleAnimation animation = new DoubleAnimation
            {
                From = start,
                To = end, // 设置鼠标进入时的边框宽度
                Duration = TimeSpan.FromSeconds(BorderAnimationTime), // 设置动画持续时间
                AccelerationRatio = BorderAnimationAccelerationRatio
            };
            target.BeginAnimation(dp, animation);
        }

        private void GridAnimation(string name, DependencyProperty dp, double start, double end)
        {
            Grid target = BT.Template.FindName(name, BT) as Grid;
            DoubleAnimation animation = new DoubleAnimation
            {
                From = start,
                To = end, // 设置鼠标进入时的边框宽度
                Duration = TimeSpan.FromSeconds(GridAnimationTime), // 设置动画持续时间
                AccelerationRatio = GridAnimationAccelerationRatio
            };
            target.BeginAnimation(dp, animation);
        }

        public void SetButtonClick(RoutedEventHandler e)
        {
            BT.Click += e;
        }
    }
}
