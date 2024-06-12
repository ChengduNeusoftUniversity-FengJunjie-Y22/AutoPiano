using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AutoPiano
{
    /// <summary>
    /// NMNAnalizeVisual.xaml 的交互逻辑
    /// </summary>
    public partial class NMNAnalizeVisual : Page
    {
        public static NMNAnalizeVisual? Instance;
        public NMNAnalizeVisual()
        {
            InitializeComponent();
            NumberedMusicalNotation.MusicScore temp = new NumberedMusicalNotation.MusicScore();
            temp.AddDefaultParagraph();
            temp.AddDefaultParagraph();
            temp.AddDefaultParagraph();
            temp.AddDefaultParagraph();
            temp.AddDefaultParagraph();
            MusicScore = temp;
            Instance = this;
        }
        private NumberedMusicalNotation.MusicScore _ms = new NumberedMusicalNotation.MusicScore();
        public NumberedMusicalNotation.MusicScore MusicScore
        {
            get { return _ms; }
            set
            {
                SongBox.Content = null;
                _ms = value;
                _ms.Update();
                SongBox.Content = _ms;
                ScrollControl.Maximum = _ms.Paragraphs.Count * 50;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SongBox.ScrollToHorizontalOffset(e.NewValue);
        }

        private void MouEnter(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Focus();
                button.Foreground = Brushes.Cyan;
                return;
            }
            if (sender is TextBox textBox)
            {
                textBox.Focus();
                textBox.Foreground = Brushes.Violet;
                return;
            }
        }
        private void MouLeave(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            if (sender is Button button)
            {
                button.Foreground = Brushes.White;
                return;
            }
            if (sender is TextBox textBox)
            {
                textBox.Focus();
                textBox.Foreground = Brushes.White;
                return;
            }
        }

        private void WhileInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var ch in e.Text)
            {
                if (!char.IsDigit(ch))
                {
                    e.Handled = true; // 阻止非数字字符的输入
                    break;
                }
            }
        }
    }
}
