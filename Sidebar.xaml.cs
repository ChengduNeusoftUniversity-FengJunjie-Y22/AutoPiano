using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
            ins1.MouseEnter += EnterBox;
            ins1.MouseLeave += LeaveBox;
            ins2.MouseEnter += EnterBox;
            ins2.MouseLeave += LeaveBox;
            ins3.MouseEnter += EnterBox;
            ins3.MouseLeave += LeaveBox;
            ins4.MouseEnter += EnterBox;
            ins4.MouseLeave += LeaveBox;
            ins5.MouseEnter += EnterBox;
            ins5.MouseLeave += LeaveBox;
            ins6.MouseEnter += EnterBox;
            ins6.MouseLeave += LeaveBox;

            ins1.Click += ClickBox;
            ins2.Click += ClickBox;
            ins3.Click += ClickBox;
            ins4.Click += ClickBox;
            ins5.Click += ClickBox;
            ins6.Click += ClickBox;
        }

        private void EnterBox(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.Cyan;
            }
        }

        private void LeaveBox(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.White;
            }
        }

        private void ClickBox(object sender, RoutedEventArgs e)
        {
            UnExpandTAB();
        }

        public void ExpandTAB()
        {
            // 创建一个双精度动画对象，用于改变按钮的宽度
            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.AccelerationRatio = 1;
            widthAnimation.From = 0; // 起始宽度
            widthAnimation.To = 320;   // 结束宽度
            widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2)); // 持续时间

            // 创建一个故事板，并将动画对象添加到其中
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);

            // 将动画应用到按钮的宽度属性
            Storyboard.SetTarget(widthAnimation, this);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

            // 启动动画
            storyboard.Begin();
            SuperMenu.IsSideBarOpen = true;
        }

        public void UnExpandTAB()
        {
            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.AccelerationRatio = 1;
            widthAnimation.From = 320; // 起始宽度
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
            SuperMenu.IsSideBarOpen = false;
        }

        private void ins1_Click(object sender, RoutedEventArgs e)
        {
            AudioBasic.UpdateAudioByType(InstrumentTypes.FWPiano);
            TxtAnalizeVisual.InstrumentType = InstrumentTypes.FWPiano;
        }

        private void ins2_Click(object sender, RoutedEventArgs e)
        {
            AudioBasic.UpdateAudioByType(InstrumentTypes.WFHorn);
            TxtAnalizeVisual.InstrumentType = InstrumentTypes.WFHorn;
        }

        private void ins3_Click(object sender, RoutedEventArgs e)
        {
            AudioBasic.UpdateAudioByType(InstrumentTypes.JHPiano);
            TxtAnalizeVisual.InstrumentType = InstrumentTypes.JHPiano;
        }

        private void ins4_Click(object sender, RoutedEventArgs e)
        {
            AudioBasic.UpdateAudioByType(InstrumentTypes.HLDrum);
            TxtAnalizeVisual.InstrumentType = InstrumentTypes.HLDrum;
        }

        private void ins5_Click(object sender, RoutedEventArgs e)
        {
            AudioBasic.UpdateAudioByType(InstrumentTypes.XMPiano);
            TxtAnalizeVisual.InstrumentType = InstrumentTypes.XMPiano;
        }
    }
}
