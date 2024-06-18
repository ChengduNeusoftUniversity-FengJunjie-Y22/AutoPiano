using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
    /// NMNAnalizeVisual.xaml 的交互逻辑
    /// </summary>
    public partial class NMNAnalizeVisual : Page
    {
        public static NMNAnalizeVisual? Instance;

        public NMNAnalizeVisual()
        {
            InitializeComponent();
            NewMusic();
            NewScore.Click = (sender, e) =>
            {
                NewMusic();
            };
            Instance = this;
        }

        private int _index = 0;
        public int Index
        {
            get => _index;
            set
            {
                if (value > -1)
                {
                    _index = value;
                }
            }
        }

        private List<TempData> _temp = new List<TempData>();
        private List<TempData> Temp
        {
            get => _temp;
            set
            {

            }
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
        public void NewMusic()
        {
            NumberedMusicalNotation.MusicScore temp = new NumberedMusicalNotation.MusicScore();
            temp.AddDefaultParagraph();
            temp.AddDefaultParagraph();
            temp.AddDefaultParagraph();
            temp.AddDefaultParagraph();
            MusicScore = temp;
        }
        public void SaveMusic()
        {
            MetaData metaData = new MetaData();
            metaData.CopyDataFrom(MusicScore);
            BinaryObject.SerializeObject<MetaData>(metaData, DataTypes.Complex_NMN, GetName());
        }
        public void ReadMusic()
        {
            var result = BinaryObject.DeserializeObject<MetaData>(DataTypes.Complex_NMN);
            if (result.Item1 && result.Item2 != null)
            {
                MusicScore = result.Item2.GetMusicScore();
            }
        }
        public string GetName()
        {
            return $"{SongName.Text} [{Index + 1}-{Index + MusicScore.Paragraphs.Count}]";
        }
        public void SortTemp()
        {

        }
        private void OutPutData(object sender, RoutedEventArgs e)
        {
            MetaData temp = new MetaData();
            temp.CopyDataFrom(MusicScore);
            if (TxtAnalizeVisual.IsNormalOutput)
            {
                StringProcessing.SaveMetaDataAsTxt(temp);
            }
            else
            {
                BinaryObject.SerializeObject<MetaData>(temp, DataTypes.Complex_NMN, (string.IsNullOrEmpty(SongName.Text) ? "Default" : SongName.Text).Replace(" ", string.Empty));
            }
        }
        private void InPutData(object sender, RoutedEventArgs e)
        {
            MetaData temp = new MetaData();
            if (TxtAnalizeVisual.IsNormalInput)
            {
                MusicScore = StringProcessing.SelectTxtThenAnalizeMeta(DataTypes.PublicMetaData).GetMusicScore();
            }
            else
            {
                var result = BinaryObject.DeserializeObject<MetaData>(DataTypes.Complex_NMN);

                if (result.Item1 && result.Item2 != null)
                {
                    temp = result.Item2;
                }

                MusicScore = temp.GetMusicScore();
            }
        }

        #region 简谱分析器主页标签控制区
        private bool _issetexpended = false;
        public bool IsSetOpen
        {
            get => _issetexpended;
            set
            {
                if (value != _issetexpended)
                {
                    _issetexpended = value;
                    if (value)
                    {
                        DoubleAnimation CloseSet = new DoubleAnimation()
                        {
                            To = 1440,
                            Duration = new Duration(TimeSpan.FromSeconds(0.3))
                        };
                        Settings.BeginAnimation(WidthProperty, CloseSet);
                    }
                    else
                    {
                        DoubleAnimation OpenSet = new DoubleAnimation()
                        {
                            To = 0,
                            Duration = new Duration(TimeSpan.FromSeconds(0.3))
                        };
                        Settings.BeginAnimation(WidthProperty, OpenSet);
                    }
                }
            }
        }
        private bool _isfileexpended = false;
        public bool IsFileSetExpended
        {
            get => _isfileexpended;
            set
            {
                if (_isfileexpended != value)
                {
                    _isfileexpended = value;
                    if (value)
                    {
                        DoubleAnimation OpenSet = new DoubleAnimation()
                        {
                            To = 280,
                            Duration = new Duration(TimeSpan.FromSeconds(0.3))
                        };
                        FileSet.BeginAnimation(HeightProperty, OpenSet);
                    }
                    else
                    {
                        DoubleAnimation CloseSet = new DoubleAnimation()
                        {
                            To = 0,
                            Duration = new Duration(TimeSpan.FromSeconds(0.3))
                        };
                        FileSet.BeginAnimation(HeightProperty, CloseSet);
                    }
                }
            }
        }
        private void SettingsBoxControl(object sender, RoutedEventArgs e)
        {
            IsSetOpen = !IsSetOpen;
        }
        private void FileSetBoxControl(object sender, RoutedEventArgs e)
        {
            IsFileSetExpended = !IsFileSetExpended;
        }
        #endregion


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


        private class TempData
        {
            public TempData() { }

            NumberedMusicalNotation.MusicScore? Data;
            string Name = string.Empty;
            int Start = 0;
            int End = 0;
        }
    }
}
