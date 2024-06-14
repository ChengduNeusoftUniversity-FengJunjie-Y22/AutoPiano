using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastHotKeyForWPF;

namespace AutoPiano
{
    /// <summary>
    /// HotKeySet.xaml 的交互逻辑
    /// </summary>
    public partial class HotKeySet : Page
    {
        public static HotKeySet? Instance;

        private static bool _isClickChange = true;
        public static bool IsClickChange
        {
            get { return _isClickChange; }
            set
            {
                _isClickChange = value;
            }
        }

        private static bool _isAutoAttentive = true;
        public static bool IsAutoAttentive
        {
            get { return _isAutoAttentive; }
            set
            {
                _isAutoAttentive = value;
            }
        }

        KeysSelectBox? k1;
        KeysSelectBox? k2;
        KeysSelectBox? k3;
        KeysSelectBox? k4;
        KeysSelectBox? k5;

        private static ComponentInfo RoundComponentInfo = new ComponentInfo()
        //例如,这条组件信息用于获取圆角组件,那你便需要设置以下属性以获取更好的效果
        {
            BorderBrush = Brushes.White,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Background = Brushes.Transparent,
            Foreground = Brushes.Cyan,
        };

        public HotKeySet()
        {
            InitializeComponent();
            Instance = this;
        }

        public static void LoadPage()
        {
            if (Instance != null)
            {
                Instance.LoadHotKeySetPage();
            }
        }

        public void LoadHotKeySetPage()
        {
            k1 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b1, RoundComponentInfo);
            k2 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b2, RoundComponentInfo);
            k3 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b3, RoundComponentInfo);
            k4 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b4, RoundComponentInfo);
            k5 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b5, RoundComponentInfo);

            BindingRef.Connect(k1, Play);
            BindingRef.Connect(k2, Pause);
            BindingRef.Connect(k3, Stop);
            BindingRef.Connect(k4, ChangePlayMode);
            BindingRef.Connect(k5, InsideVisual);

            k1.CurrentKeyA = Key.LeftCtrl;
            k1.CurrentKeyB = Key.A;

            k2.CurrentKeyA = Key.LeftCtrl;
            k2.CurrentKeyB = Key.S;

            k3.CurrentKeyA = Key.LeftCtrl;
            k3.CurrentKeyB = Key.D;

            k4.CurrentKeyA = Key.LeftAlt;
            k4.CurrentKeyB = Key.C;

            k5.CurrentKeyA = Key.LeftAlt;
            k5.CurrentKeyB = Key.V;
        }

        public static void Play()
        {
            Pause();
            switch (EditArea.PageType)
            {
                case PageTypes.TxtAnalize:
                    TxtAnalizeVisual.CurrentSong.Model = PlayModel.Preview;
                    TxtAnalizeVisual.CurrentSong.Start();
                    break;
                case PageTypes.NMNAnalize:
                    NMNAnalizeVisual.Instance?.MusicScore.Start();
                    break;
            }
        }
        public static void ChangePlayMode()
        {
            Pause();
            TxtAnalizeVisual.CurrentSong.Model = (TxtAnalizeVisual.CurrentSong.Model == PlayModel.Auto ? PlayModel.Preview : PlayModel.Auto);
        }

        public static void Pause()
        {
            TxtAnalizeVisual.CurrentSong.Pause();
            NMNAnalizeVisual.Instance?.MusicScore.Stop();
        }
        public static void Stop()
        {
            TxtAnalizeVisual.CurrentSong.Stop();
            NMNAnalizeVisual.Instance?.MusicScore.Stop();
        }
        public static void InsideVisual()
        {

        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.Cyan;
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.White;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsClickChange = !IsClickChange;
            PageChangeMode.ButtonText = (IsClickChange ? "点击" : "滑动");
        }

        private void AttentiveMode_Click(object sender, RoutedEventArgs e)
        {
            IsAutoAttentive = !IsAutoAttentive;
            AttentiveChangeMode.ButtonText = (IsAutoAttentive ? "ON" : "OFF");
        }

        private void AnalizeMode(object sender, RoutedEventArgs e)
        {
            TxtAnalizeVisual.IsNormalInput = !TxtAnalizeVisual.IsNormalInput;
            ReadMode.ButtonText = (TxtAnalizeVisual.IsNormalInput ? "通用" : "内部");
        }

        private void OutputMode(object sender, RoutedEventArgs e)
        {
            TxtAnalizeVisual.IsNormalOutput = !TxtAnalizeVisual.IsNormalOutput;
            WriteMode.ButtonText = (TxtAnalizeVisual.IsNormalOutput ? "通用" : "内部");
        }
    }
}
