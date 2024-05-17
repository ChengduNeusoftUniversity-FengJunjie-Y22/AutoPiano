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
        public static bool IsMenuLocked = false;

        public SuperMenu()
        {
            InitializeComponent();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            IsMenuLocked = true;
            MenuBox.Foreground = Brushes.Cyan;
            if (Sidebar.Instance != null)
            {
                Sidebar.Instance.ExpandTAB();
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuBox.Foreground = Brushes.White;
            if (IsMenuLocked) { IsMenuLocked = false; return; }
            if (Sidebar.Instance != null)
            {
                Sidebar.Instance.UnExpandTAB();
            }
        }

        private void WindowClose_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowClose.Foreground = Brushes.Red;
        }

        private void MinSize_MouseEnter(object sender, MouseEventArgs e)
        {
            MinSize.Foreground = Brushes.Red;
        }

        private void MidelSize_MouseEnter(object sender, MouseEventArgs e)
        {
            MidelSize.Foreground = Brushes.Red;
        }

        private void WindowClose_MouseLeave(object sender, MouseEventArgs e)
        {
            WindowClose.Foreground = Brushes.White;
        }

        private void MidelSize_MouseLeave(object sender, MouseEventArgs e)
        {
            MidelSize.Foreground = Brushes.White;
        }

        private void MinSize_MouseLeave(object sender, MouseEventArgs e)
        {
            MinSize.Foreground = Brushes.White;
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
    }
}
