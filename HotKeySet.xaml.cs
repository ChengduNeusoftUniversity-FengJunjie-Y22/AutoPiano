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

        public bool IsSameDataMode = true;//两种协议间是否自动同步

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
            //CTRL保护区
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.A);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.C);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.V);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.X);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.S);

            //ALT保护区
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.S);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.Z);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F1);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F2);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F3);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F4);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F5);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F6);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F7);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F9);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F10);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.F11);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.ALT, NormalKeys.R);

            k1 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b1, RoundComponentInfo);
            k2 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b2, RoundComponentInfo);
            k3 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b3, RoundComponentInfo);
            k4 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b4, RoundComponentInfo);
            k5 = PrefabComponent.SetAsRoundBox<KeysSelectBox>(b5, RoundComponentInfo);

            BindingRef.Connect(k1, Play);
            BindingRef.Connect(k2, Pause);
            BindingRef.Connect(k3, Stop);
            BindingRef.Connect(k4, HideGameVisual);
            BindingRef.Connect(k5, InsideVisual);

            k1.CurrentKeyA = Key.LeftCtrl;
            k1.CurrentKeyB = Key.Left;

            k2.CurrentKeyA = Key.LeftCtrl;
            k2.CurrentKeyB = Key.Down;

            k3.CurrentKeyA = Key.LeftCtrl;
            k3.CurrentKeyB = Key.Right;

            k4.CurrentKeyA = Key.LeftAlt;
            k4.CurrentKeyB = Key.C;

            k5.CurrentKeyA = Key.LeftAlt;
            k5.CurrentKeyB = Key.V;

            k1.UseFailureTrigger(FailRegis);
            k2.UseFailureTrigger(FailRegis);
            k3.UseFailureTrigger(FailRegis);
            k4.UseFailureTrigger(FailRegis);
            k5.UseFailureTrigger(FailRegis);
        }

        public static void Play()
        {
            switch (EditArea.PageType)
            {
                case PageTypes.TxtAnalize:
                    TxtAnalizeVisual.CurrentSong.Start();
                    break;
                case PageTypes.NMNAnalize:
                    NMNAnalizeVisual.Instance?.MusicScore.Start();
                    break;
            }
        }
        public static void HideGameVisual()
        {
            TxtAnalizeVisual.PopupX.HideControl();
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
            TxtAnalizeVisual.PopupControl = !TxtAnalizeVisual.PopupControl;
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
            ReadMode.ButtonText = (TxtAnalizeVisual.IsNormalInput ? "Public" : "Private");
            if (IsSameDataMode)
            {
                TxtAnalizeVisual.IsNormalOutput = TxtAnalizeVisual.IsNormalInput;
                WriteMode.ButtonText = (TxtAnalizeVisual.IsNormalOutput ? "Public" : "Private");
            }
        }

        private void OutputMode(object sender, RoutedEventArgs e)
        {
            TxtAnalizeVisual.IsNormalOutput = !TxtAnalizeVisual.IsNormalOutput;
            WriteMode.ButtonText = (TxtAnalizeVisual.IsNormalOutput ? "Public" : "Private");
            if (IsSameDataMode)
            {
                TxtAnalizeVisual.IsNormalInput = TxtAnalizeVisual.IsNormalOutput;
                ReadMode.ButtonText = (TxtAnalizeVisual.IsNormalInput ? "Public" : "Private");
            }
        }

        private void SameDMode(object sender, RoutedEventArgs e)
        {
            IsSameDataMode = !IsSameDataMode;
            SameDM.ButtonText = (IsSameDataMode ? "On" : "OFF");
        }

        private void FailRegis(object sender)
        {
            if (sender is KeysSelectBox e)
            {
                e.Text = e.DefaultErrorText;
            }
        }
    }
}
