using System;
using System.Collections;
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
                    IndexString.Text = _index.ToString();
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
                _ms.UpdateCoresAfterUILoaded();
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
            FileTool.SerializeObject<MetaData>(metaData, DataTypes.PrivateVisualData, GetName());
        }
        public void ReadMusic()
        {
            var result = FileTool.DeserializeObject<MetaData>(DataTypes.PrivateVisualData);
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
            NewTip("输出中");
            MetaData temp = new MetaData();
            temp.CopyDataFrom(MusicScore);
            if (TxtAnalizeVisual.IsNormalOutput)
            {
                bool result = FileTool.SerializeObject<MetaData>(temp, DataTypes.PublicVisualData, GetName());
                NewTip("【Public】数据已输出完成", 1500);
                if (!result) return;
            }
            else
            {
                bool result = FileTool.SerializeObject<MetaData>(temp, DataTypes.PrivateVisualData, (string.IsNullOrEmpty(SongName.Text) ? "Default" : SongName.Text).Replace(" ", string.Empty));
                NewTip("【Private】数据已输出完成", 1500);
                if (!result) return;
            }
            NewTip("⚠未能正确输出为文件", 1500);
        }
        private void InPutData(object sender, RoutedEventArgs e)
        {
            NewTip("选择文件中", 1500);
            if (TxtAnalizeVisual.IsNormalInput)
            {
                var result = FileTool.DeserializeObject<MetaData>(DataTypes.PublicVisualData);
                if (result.Item1) { MusicScore = result.Item2.GetMusicScore(); NewTip("读取完毕", 1500); return; }
            }
            else
            {
                var result = FileTool.DeserializeObject<MetaData>(DataTypes.PrivateVisualData);
                if (result.Item1) { MusicScore = result.Item2.GetMusicScore(); NewTip("读取完毕", 1500); return; }
            }
            NewTip("⚠未能读取到数据", 1500);
        }
        private void ClearTempObject(object sender, RoutedEventArgs e)
        {
            PrivateObjects.Children.Clear();
            Index = 0;
            NewTip("⚠已清除工作簿", 1500);
        }
        private void AddTempObject(object sender, RoutedEventArgs e)
        {
            TempData tempData = new TempData();
            tempData.Name = SongName.Text;
            tempData.ButtonText = tempData.GetFullName(Index + 1, Index + MusicScore.Paragraphs.Count);
            tempData.Data = MusicScore;
            tempData.Start = Index + 1;
            tempData.End = Index + MusicScore.Paragraphs.Count;
            Index += MusicScore.Paragraphs.Count;
            PrivateObjects.Children.Add(tempData);
            NewTip("添加了一个工作簿√", 1500);
        }
        private void SumTempToTxtAnalize(object sender, RoutedEventArgs e)
        {
            TxtAnalizeVisual.CurrentSong = SumSong();
            NewTip("已合成并输出至【文本解析器】", 1500);
        }
        private void SumTempToFloder(object sender, RoutedEventArgs e)
        {
            foreach (var item in PrivateObjects.Children)
            {
                if (item is TempData data)
                {
                    MetaData metaData = new MetaData();
                    if (data.Data != null) { metaData.CopyDataFrom(data.Data); }
                    if (TxtAnalizeVisual.IsNormalOutput)
                    {
                        string floderPath = System.IO.Path.Combine(FileTool.PublicVisualData, data.Name);
                        if (!System.IO.Directory.Exists(floderPath))
                        {
                            System.IO.Directory.CreateDirectory(floderPath);
                        }
                        FileTool.SerializeObject(metaData, System.IO.Path.Combine(floderPath, data.GetFullName() + ".txt"), true);
                        NewTip($"已合成并输出至【{floderPath}】", 4000);
                    }
                    else
                    {
                        string floderPath = System.IO.Path.Combine(FileTool.PrivateVisualData, data.Name);
                        if (!System.IO.Directory.Exists(floderPath))
                        {
                            System.IO.Directory.CreateDirectory(floderPath);
                        }
                        FileTool.SerializeObject(metaData, System.IO.Path.Combine(floderPath, data.GetFullName() + ".bin"), false);
                        NewTip($"已合成并输出至【{floderPath}】", 4000);
                    }
                }
            }
        }
        private void AddFromEnd(object sender, RoutedEventArgs e)
        {
            MusicScore.AddDefaultParagraph();
            NewTip("从末尾处增加了一个小节√", 1000);
        }
        private void DeleteFromEnd(object sender, RoutedEventArgs e)
        {
            MusicScore.DeleteLastParagraph();
            NewTip("⚠从末尾处删除了一个小节", 1000);
        }

        private Song SumSong()
        {
            Song song = new Song();
            song.Name = SongName.Text;
            List<NumberedMusicalNotation.MusicScore> score = new List<NumberedMusicalNotation.MusicScore>();
            List<string> names = SlipName();


            foreach (string name in names)
            {
                List<TempData> tempDatas = new List<TempData>();
                foreach (var item in PrivateObjects.Children)
                {
                    if (item is TempData info && name == info.Name)
                    {
                        tempDatas.Add(info);
                    }
                }
                tempDatas = tempDatas.OrderBy(info => info.Start).ToList();
                foreach (TempData tempData in tempDatas)
                {
                    song += tempData.Data == null ? new Song() : tempData.Data.ConvertToSong();
                }
            }

            return song;
        }

        private List<string> SlipName()
        {
            List<string> list = new List<string>();

            foreach (var item in PrivateObjects.Children)
            {
                if (item is TempData info && !list.Contains(info.Name))
                {
                    list.Add(info.Name);
                }
            }

            return list;
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
                            To = 350,
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
        public void NewTip(string info)
        {
            EditInfo.Text = info;
        }
        public async void NewTip(string info, int span)
        {
            EditInfo.Text = info;
            await Task.Delay(span);
            EditInfo.Text = string.Empty;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Index = StringProcessing.StringToInt(IndexString.Text);
        }

        private void Speed_TextChanged(object sender, TextChangedEventArgs e)
        {
            MusicTheory.Speed = StringProcessing.StringToInt(Speed.Text);
            if (Speed.Text != MusicTheory.Speed.ToString())
            {
                NewTip($"⚠无法将值设为【{Speed.Text}】，已恢复默认", 2000);
                Speed.Text = MusicTheory.Speed.ToString();
            }
        }

        private void Left_TextChanged(object sender, TextChangedEventArgs e)
        {
            MusicTheory.LeftNum = StringProcessing.StringToInt(Left.Text);
            if (Left.Text != MusicTheory.LeftNum.ToString())
            {
                NewTip($"⚠无法将值设为【{Speed.Text}】，已恢复默认", 2000);
                Left.Text = MusicTheory.LeftNum.ToString();
            }
        }

        private void Right_TextChanged(object sender, TextChangedEventArgs e)
        {
            MusicTheory.RightNum = StringProcessing.StringToInt(Right.Text);
            if (Right.Text != MusicTheory.RightNum.ToString())
            {
                NewTip($"⚠无法将值设为【{Speed.Text}】，已恢复默认", 2000);
                Right.Text = MusicTheory.RightNum.ToString();
            }
        }

        private void SetToDefaultTheory(object sender, RoutedEventArgs e)
        {
            Speed.Text = "80";
            Left.Text = "4";
            Right.Text = "4";
            NewTip("已恢复默认", 4000);
        }

        public class TempData : ButtonX
        {
            public TempData()
            {
                Width = double.NaN;
                ButtonTextColor = Brushes.White;
                BorderAnimationSide = new Thickness(0, 0, 0, 1);
                BorderAnimationColor = Brushes.Lime;
                HoverTextColor = Brushes.Lime;
                ButtonTextSize = 30;
                Height = 50;
                Click = (sender, e) =>
                {
                    if (Instance != null && Data != null)
                    {
                        Instance.MusicScore = Data;
                    }
                };
                PreviewMouseRightButtonDown += (sender, e) =>
                {
                    StackPanel? father = Parent as StackPanel;
                    TempData? tempData = null;
                    if (father != null)
                    {
                        foreach (var item in father.Children)
                        {
                            if (item == this)
                            {
                                tempData = this;
                            }
                        }
                        if (tempData != null) { father.Children.Remove(tempData); }
                    }
                };
            }

            public NumberedMusicalNotation.MusicScore? Data;
            public string Name = string.Empty;
            public int Start = 0;
            public int End = 0;

            public string GetFullName()
            {
                return Name + $"  【 {Start} - {End} 】  ";
            }
            public string GetFullName(int start, int end)
            {
                return Name + $"  【 {start} - {end} 】  ";
            }
        }
    }
}
