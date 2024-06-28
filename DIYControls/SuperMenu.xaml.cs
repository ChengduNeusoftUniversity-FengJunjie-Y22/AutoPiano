using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace AutoPiano
{
    /// <summary>
    /// SuperMenu.xaml 的交互逻辑
    /// </summary>
    public partial class SuperMenu : Border
    {
        public static bool IsSideBarOpen = false;

        public SuperMenu()
        {
            InitializeComponent();
            WindowClose.MouseEnter += SizeModeEnter;
            WindowClose.MouseLeave += SizeModeLeave;
            MinSize.MouseEnter += SizeModeEnter;
            MinSize.MouseLeave += SizeModeLeave;
            MidelSize.MouseEnter += SizeModeEnter;
            MidelSize.MouseLeave += SizeModeLeave;
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

        private void SizeModeEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.Red;
            }
        }

        private void SizeModeLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.White;
            }
        }

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void MidelSize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void MinSize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MenuBox1_Click(object sender, RoutedEventArgs e)
        {
            ReverseSideTab();
        }

        private void MenuBox2_Click(object sender, RoutedEventArgs e)
        {
            if (HotKeySet.IsClickChange && EditArea.Instance != null)
            {
                EditArea.PageType = PageTypes.TxtAnalize;
            }
        }

        private void MenuBox3_Click(object sender, RoutedEventArgs e)
        {
            if (HotKeySet.IsClickChange && EditArea.Instance != null)
            {
                EditArea.PageType = PageTypes.NMNAnalize;
            }
        }

        private void MenuBox4_Click(object sender, RoutedEventArgs e)
        {
            if (HotKeySet.IsClickChange && EditArea.Instance != null)
            {
                EditArea.PageType = PageTypes.HotKeySet;
            }
        }

        public static void ReverseSideTab()
        {
            if (Sidebar.Instance != null)
            {
                if (IsSideBarOpen)
                {
                    Sidebar.Instance.UnExpandTAB();
                }
                else
                {
                    Sidebar.Instance.ExpandTAB();
                }
            }
        }

        public static void CloseSideTab()
        {
            if (Sidebar.Instance != null && IsSideBarOpen)
            {
                Sidebar.Instance.UnExpandTAB();
            }
        }

        private void MenuBox2_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseSideTab();
            if (sender is Button button)
            {
                button.Foreground = Brushes.Cyan;
            }
            if (EditArea.Instance != null && !HotKeySet.IsClickChange)
            {
                EditArea.PageType = PageTypes.TxtAnalize;
            }
        }

        private void MenuBox3_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseSideTab();
            if (sender is Button button)
            {
                button.Foreground = Brushes.Cyan;
            }
            if (EditArea.Instance != null && !HotKeySet.IsClickChange)
            {
                EditArea.PageType = PageTypes.NMNAnalize;
            }
        }

        private void MenuBox4_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseSideTab();
            if (sender is Button button)
            {
                button.Foreground = Brushes.Cyan;
            }
            if (EditArea.Instance != null && !HotKeySet.IsClickChange)
            {
                EditArea.PageType = PageTypes.HotKeySet;
            }
        }
    }
}
