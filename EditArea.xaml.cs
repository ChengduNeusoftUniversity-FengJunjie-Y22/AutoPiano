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
    /// EditArea.xaml 的交互逻辑
    /// </summary>
    public partial class EditArea : Border
    {
        public static EditArea? Instance;

        public static TxtAnalizeVisual? VisualA;
        public static NMNAnalizeVisual? VisualB;
        public static HotKeySet? VisualC;

        private static PageTypes _pageType = PageTypes.TxtAnalize;

        /// <summary>
        /// 切页效果的其实位置
        /// </summary>
        public static double StartValue = 0;

        /// <summary>
        /// 更改此属性将直接切换页面
        /// </summary>
        public static PageTypes PageType
        {
            get { return _pageType; }
            set
            {
                switch (_pageType)
                {
                    case PageTypes.TxtAnalize:
                        StartValue = 0;
                        break;
                    case PageTypes.NMNAnalize:
                        StartValue = -1440;
                        break;
                    case PageTypes.HotKeySet:
                        StartValue = -2880;
                        break;
                }
                _pageType = value;
                Instance?.ChangePage();
            }
        }

        public EditArea()
        {
            InitializeComponent();
            Instance = this;
            VisualA = new TxtAnalizeVisual();
            VisualB = new NMNAnalizeVisual();
            VisualC = new HotKeySet();
            WorkAreaA.Navigate(VisualA);
            WorkAreaB.Navigate(VisualB);
            WorkAreaC.Navigate(VisualC);
        }

        /// <summary>
        /// 切换页面的动画效果实现，已在属性中被实时调用
        /// </summary>
        public void ChangePage()
        {
            double Offest = 0;

            switch (_pageType)
            {
                case PageTypes.TxtAnalize:
                    Offest = 1440f * 0f;
                    break;
                case PageTypes.NMNAnalize:
                    Offest = -1440f * 1f;
                    break;
                case PageTypes.HotKeySet:
                    Offest = -1440f * 2f;
                    break;
            }

            // 创建一个 TranslateTransform，并将其应用到ScrollViewer的Content属性
            TranslateTransform translateTransform = new TranslateTransform();
            PageControl.RenderTransform = translateTransform;
            // 创建一个 DoubleAnimation 实例
            DoubleAnimation animation = new DoubleAnimation();
            animation.AccelerationRatio = 1;
            animation.From = StartValue;
            animation.To = Offest;
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.4)); // 动画持续时间
            // 将动画与 TranslateTransform 的 X 属性绑定
            translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);


            if (VisualA != null && VisualB != null && VisualC != null)
            {
                DoubleAnimation widthAnimation1 = new DoubleAnimation();
                widthAnimation1.AccelerationRatio = 1;
                widthAnimation1.From = 1440; // 起始宽度
                widthAnimation1.To = 300;   // 结束宽度
                widthAnimation1.Duration = new Duration(TimeSpan.FromSeconds(2)); // 持续时间
                DoubleAnimation widthAnimation2 = new DoubleAnimation();
                widthAnimation2.AccelerationRatio = 1;
                widthAnimation2.From = 1440; // 起始宽度
                widthAnimation2.To = 300;   // 结束宽度
                widthAnimation2.Duration = new Duration(TimeSpan.FromSeconds(2)); // 持续时间
                DoubleAnimation widthAnimation3 = new DoubleAnimation();
                widthAnimation3.AccelerationRatio = 1;
                widthAnimation3.From = 1440; // 起始宽度
                widthAnimation3.To = 300;   // 结束宽度
                widthAnimation3.Duration = new Duration(TimeSpan.FromSeconds(2)); // 持续时间


                // 创建一个故事板，并将动画对象添加到其中
                Storyboard storyboard1 = new Storyboard();
                storyboard1.Children.Add(widthAnimation1);
                // 将动画应用到按钮的宽度属性
                Storyboard.SetTarget(widthAnimation1, VisualA.ModeTXT);
                Storyboard.SetTargetProperty(widthAnimation1, new PropertyPath(WidthProperty));

                Storyboard storyboard2 = new Storyboard();
                storyboard2.Children.Add(widthAnimation2);
                Storyboard.SetTarget(widthAnimation2, VisualB.ModeVisual);
                Storyboard.SetTargetProperty(widthAnimation2, new PropertyPath(WidthProperty));

                // 创建一个故事板，并将动画对象添加到其中
                Storyboard storyboard3 = new Storyboard();
                storyboard3.Children.Add(widthAnimation3);
                Storyboard.SetTarget(widthAnimation3, VisualC.ModeHotKey);
                Storyboard.SetTargetProperty(widthAnimation3, new PropertyPath(WidthProperty));

                // 启动动画
                storyboard1.Begin();
                storyboard2.Begin();
                storyboard3.Begin();
            }
        }
    }
}
