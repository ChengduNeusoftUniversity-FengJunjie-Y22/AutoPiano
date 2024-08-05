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

        public HotKeySet()
        {
            InitializeComponent();
            Instance = this;
        }

        public void Play(object sender, HotKeyEventArgs e)
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
        public void HideGameVisual(object sender, HotKeyEventArgs e)
        {
            TxtAnalizeVisual.PopupX.HideControl();
        }

        public void Pause(object sender, HotKeyEventArgs e)
        {
            TxtAnalizeVisual.CurrentSong.Pause();
            NMNAnalizeVisual.Instance?.MusicScore.Stop();
        }
        public void Stop(object sender, HotKeyEventArgs e)
        {
            TxtAnalizeVisual.CurrentSong.Stop();
            NMNAnalizeVisual.Instance?.MusicScore.Stop();
        }
        public void InsideVisual(object sender, HotKeyEventArgs e)
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

        public void UpdateAfterUseTemp()
        {
            PageChangeMode.ButtonText = (IsClickChange ? "点击" : "滑动");
            AttentiveChangeMode.ButtonText = (IsAutoAttentive ? "ON" : "OFF");
            ReadMode.ButtonText = (TxtAnalizeVisual.IsNormalInput ? "Public" : "Private");
            WriteMode.ButtonText = (TxtAnalizeVisual.IsNormalOutput ? "Public" : "Private");
            SameDM.ButtonText = (IsSameDataMode ? "On" : "OFF");
        }
    }
}
