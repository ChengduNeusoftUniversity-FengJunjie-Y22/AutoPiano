using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Printing;
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
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AutoPiano.NumberedMusicalNotation;
using System.Reflection;

namespace AutoPiano
{
    /// <summary>
    /// 文本乐谱的交互界面
    /// </summary>
    public partial class TxtAnalizeVisual : Page
    {
        public static TxtAnalizeVisual? Instance;

        public static InstrumentTypes _instrumentType = InstrumentTypes.FWPiano;
        /// <summary>
        /// 预览所使用的乐器类型
        /// </summary>
        public static InstrumentTypes InstrumentType
        {
            get { return _instrumentType; }
            set
            {
                _instrumentType = value;
                if (Instance != null)
                {
                    switch (_instrumentType)
                    {
                        case InstrumentTypes.FWPiano:
                            Instance.AudioType.Text = "风物之诗琴";
                            break;
                        case InstrumentTypes.WFHorn:
                            Instance.AudioType.Text = "晚风圆号";
                            break;
                        case InstrumentTypes.JHPiano:
                            Instance.AudioType.Text = "镜花之琴";
                            break;
                        case InstrumentTypes.XMPiano:
                            Instance.AudioType.Text = "老旧的诗琴";
                            break;
                        case InstrumentTypes.HLDrum:
                            Instance.AudioType.Text = "荒泷盛世豪鼓";
                            break;
                    }
                }
            }
        }

        private static Song _data = new Song();
        /// <summary>
        /// 当前界面存放的曲目
        /// </summary>
        public static Song CurrentSong
        {
            get { return _data; }
            set
            {
                _data = value;
                if (Instance != null)
                {
                    Instance.SongName.Text = _data.Name;
                    _data.Model = PlayModel.Preview;
                    Instance.TimeValue.Text = string.Empty;
                    Instance.Notes.Children.Clear();
                    foreach (StackPanel textBlock in Instance.LoadPanelBoxes())
                    {
                        Instance.Notes.Children.Add(textBlock);
                    }
                    _data.Pause();
                    _data.Position = 0;
                    _data.IsOnPlaying = false;
                    _data.IsStop = false;
                }
            }
        }

        public static bool IsAttentive = false;//是否处于专注

        public static bool IsPreviewSingleOne = false;

        public static bool IsOnSlider//鼠标是否位于进度拖条上
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.IsMouseInSlider;
                }
                return false;
            }
        }

        public static Slider Slider
        {
            get
            {
                if (Instance != null) { return Instance.ProgressSlider; }
                return new Slider();
            }
            set
            {
                if (Instance != null) { Instance.ProgressSlider = value; }
            }
        }

        public TxtAnalizeVisual()
        {
            InitializeComponent();
            Instance = this;
        }


        #region 拖条处理
        public bool IsMouseInSlider = false;
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsMouseInSlider) { CurrentSong.Position = (int)(e.NewValue * (CurrentSong.notes.Count - 1)); }
        }
        private void PSliderMouseEnter(object sender, MouseEventArgs e)
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); }
            IsMouseInSlider = true;
        }
        private void PSliderMouseLeave(object sender, MouseEventArgs e)
        {
            IsMouseInSlider = false;
        }
        #endregion


        #region 解析器功能与设置
        private async void Button_Click(object sender, RoutedEventArgs e)//Txt解析
        {
            CurrentSong.Stop();
            var result = await StringProcessing.SelectThenAnalize();
            CurrentSong = result.Item1;
            SongName.Text = result.Item2;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)//粘贴板解析
        {
            CurrentSong.Stop();
            CurrentSong = await StringProcessing.SongParse(Clipboard.GetText());
            SongName.Text = "? ? ?";
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)//一个空格所代表的时值
        {
            SpaceValue.Text = "当前 : " + (int)e.NewValue;
            StringProcessing.BlankSpace = (int)e.NewValue;
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)//微调时的变化量
        {
            AdjustValue.Text = "当前 : " + (int)e.NewValue;
            StringProcessing.BlankSpace_Re = (int)e.NewValue;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)//存档
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); }
            if (string.IsNullOrEmpty(SongName.Text)) { MessageBox.Show("名称不可为空!"); return; }
            SaveButton.Content = "写入ing……";
            SaveButton.Foreground = Brushes.Lime;
            CurrentSong.Name = SongName.Text;
            if (BinaryObject.SerializeObject(CurrentSong, DataTypes.Simple, CurrentSong.Name))
            {
                SaveButton.Content = "完成√";
                await Task.Delay(2500);
                SaveButton.Content = "存档";
                SaveButton.Foreground = Brushes.White;
            }
            else
            {
                SaveButton.Foreground = Brushes.Red;
                SaveButton.Content = "⚠失败";
                await Task.Delay(2500);
                SaveButton.Content = "存档";
                SaveButton.Foreground = Brushes.White;
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)//读档
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); }
            ReadButton.Content = "读取ing……";
            ReadButton.Foreground = Brushes.Lime;
            var result = BinaryObject.DeserializeObject<Song>();
            if (result.Item1)
            {
                if (result.Item2 != null) { CurrentSong = result.Item2; }
                ReadButton.Content = "完成√";
                await Task.Delay(2500);
                ReadButton.Content = "读档";
                ReadButton.Foreground = Brushes.White;
            }
            else
            {
                ReadButton.Foreground = Brushes.Red;
                ReadButton.Content = "⚠失败";
                await Task.Delay(2500);
                ReadButton.Content = "读档";
                ReadButton.Foreground = Brushes.White;
            }
        }
        #endregion


        #region 其它可视化区
        private StackPanel[] LoadPanelBoxes()//加载音符显示区
        {
            StackPanel[] result = new StackPanel[CurrentSong.notes.Count];
            for (int i = 0; i < CurrentSong.notes.Count; i++)
            {
                string notestr = string.Empty;

                if (CurrentSong.notes[i] is Note note)
                {
                    notestr = note.GetContentWithOutTime();
                }
                else if (CurrentSong.notes[i] is Chord chord)
                {
                    notestr = chord.GetContentWithOutTime();
                }
                else if (CurrentSong.notes[i] is NullNote nullnote)
                {
                    notestr = nullnote.GetContentWithOutTime();
                }

                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 60,
                };

                foreach (char item in notestr)
                {
                    TextBlock textBlock = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                        Background = Brushes.Transparent,
                        Padding = new Thickness(0),
                        LineHeight = 0.1,
                        FontSize = 28,
                        Width = 40,
                        Foreground = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text = item.ToString(),
                    };
                    Border border = new Border()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 60,
                        BorderBrush = Brushes.White,
                        BorderThickness = new Thickness(0, 0, 1, 0)
                    };
                    border.Child = textBlock;
                    stackPanel.Children.Add(border);
                }


                result[i] = stackPanel;
            }

            return result;
        }
        public static async void ColorChange(int index, int time, string info)
        {
            if (Instance != null)
            {
                Instance.TimeValue.Text = info;
                if (Instance.Notes.Children[index] is StackPanel spA)
                {
                    foreach (var item in spA.Children)
                    {
                        if (item is Border border)
                        {
                            if (border.Child is TextBlock textBlock)
                            {
                                textBlock.Foreground = Brushes.Cyan;
                            }
                        }
                    }
                    await Task.Delay(time);
                    foreach (var item in spA.Children)
                    {
                        if (item is Border border)
                        {
                            if (border.Child is TextBlock textBlock)
                            {
                                textBlock.Foreground = Brushes.White;
                            }
                        }
                    }
                }
            }
        }
        public static void WhiteColor(int target, SolidColorBrush color)
        {
            if (Instance != null && target < CurrentSong.notes.Count)
            {
                if (Instance.Notes.Children[target] is StackPanel spA)
                {
                    foreach (var item in spA.Children)
                    {
                        if (item is Border border)
                        {
                            if (border.Child is TextBlock textBlock)
                            {
                                textBlock.Foreground = color;
                            }
                        }
                    }
                }
            }
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (IsAttentive)
            {
                DoubleAnimation widthAnimation = new DoubleAnimation();
                widthAnimation.AccelerationRatio = 1;
                widthAnimation.From = 1440; // 起始宽度
                widthAnimation.To = 0;   // 结束宽度
                widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4)); // 持续时间

                // 创建一个故事板，并将动画对象添加到其中
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(widthAnimation);

                // 将动画应用到按钮的宽度属性
                Storyboard.SetTarget(widthAnimation, TimeEdit);
                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

                // 启动动画
                storyboard.Begin();
                IsAttentive = false;
            }
            else
            {
                DoubleAnimation widthAnimation = new DoubleAnimation();
                widthAnimation.AccelerationRatio = 1;
                widthAnimation.From = 0; // 起始宽度
                widthAnimation.To = 1440;   // 结束宽度
                widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4)); // 持续时间

                // 创建一个故事板，并将动画对象添加到其中
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(widthAnimation);

                // 将动画应用到按钮的宽度属性
                Storyboard.SetTarget(widthAnimation, TimeEdit);
                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

                // 启动动画
                storyboard.Begin();
                IsAttentive = true;
            }
        }
        public static void UnExpendBox()
        {
            if (Instance != null && IsAttentive)
            {
                DoubleAnimation widthAnimation = new DoubleAnimation();
                widthAnimation.AccelerationRatio = 1;
                widthAnimation.From = 1440; // 起始宽度
                widthAnimation.To = 0;   // 结束宽度
                widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4)); // 持续时间

                // 创建一个故事板，并将动画对象添加到其中
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(widthAnimation);

                // 将动画应用到按钮的宽度属性
                Storyboard.SetTarget(widthAnimation, Instance.TimeEdit);
                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

                // 启动动画
                storyboard.Begin();
                IsAttentive = false;
            }
        }
        public static void ExpendBox()
        {
            if (Instance != null && !IsAttentive)
            {
                DoubleAnimation widthAnimation = new DoubleAnimation();
                widthAnimation.AccelerationRatio = 1;
                widthAnimation.From = 0; // 起始宽度
                widthAnimation.To = 1440;   // 结束宽度
                widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4)); // 持续时间

                // 创建一个故事板，并将动画对象添加到其中
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(widthAnimation);

                // 将动画应用到按钮的宽度属性
                Storyboard.SetTarget(widthAnimation, Instance.TimeEdit);
                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

                // 启动动画
                storyboard.Begin();
                IsAttentive = true;
            }
        }
        public void NewInfo(object target)
        {
            AIndex.Text = CurrentSong.Position.ToString();
            if (target is Note note)
            {
                AKey.Text = note.GetContentWithOutTime();
                ATime.Text = note.Span.ToString();
                TimeValue.Text = note.GetContent();
                if (IsPreviewSingleOne) { note.Preview(); IsPreviewSingleOne = false; }
                return;
            }
            if (target is Chord chord)
            {
                AKey.Text = chord.GetContentWithOutTime();
                ATime.Text = chord.Chords.Last().Span.ToString();
                TimeValue.Text = chord.GetContent();
                if (IsPreviewSingleOne) { chord.Preview(); IsPreviewSingleOne = false; }
                return;
            }
            if (target is NullNote nunote)
            {
                AKey.Text = nunote.GetContentWithOutTime();
                ATime.Text = nunote.Span.ToString();
                TimeValue.Text = nunote.GetContent();
                if (IsPreviewSingleOne) { nunote.Preview(); IsPreviewSingleOne = false; }
                return;
            }
        }
        #endregion


        #region 按钮事件--时值编辑器
        private async void AddNewParagraph(object sender, RoutedEventArgs e)//从当前位置替换一段新的解析结果
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); }
            string result = StringProcessing.SelectThenReadTxt();

            CurrentSong.AddParagraph(result);
            Song temp = Song.Copy(CurrentSong);
            CurrentSong = temp;

            ReadButton.Content = "完成√";
            await Task.Delay(2500);
            ReadButton.Content = "读档";
            ReadButton.Foreground = Brushes.White;
        }
        private void ClearNote(object sender, RoutedEventArgs e)//清除当前位置的音符
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            CurrentSong.AddParagraph(" ");
            Song temp = Song.Copy(CurrentSong);
            CurrentSong = temp;
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)//加速
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            object temp = CurrentSong.notes[CurrentSong.Position];
            if (temp is Note note)
            {
                int a = note.Span - StringProcessing.BlankSpace / 2;
                if (a >= 0)
                {
                    note.Span = a;
                }
                else
                {
                    note.Span = 0;
                }
            }
            else if (temp is Chord chord)
            {
                int a = chord.Chords.Last().Span - StringProcessing.BlankSpace / 2;
                if (a >= 0)
                {
                    chord.Chords.Last().Span = a;
                }
                else
                {
                    chord.Chords.Last().Span = 0;
                }
            }
            else if (temp is NullNote nunote)
            {
                int a = nunote.Span - StringProcessing.BlankSpace / 2;
                if (a >= 0)
                {
                    nunote.Span = a;
                }
                else
                {
                    nunote.Span = 0;
                }
            }
            CurrentSong.Position = CurrentSong.Position;
        }
        private void Button_Click_9(object sender, RoutedEventArgs e)//降速
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            object temp1 = CurrentSong.notes[CurrentSong.Position];
            if (temp1 is Note note1)
            {
                note1.Span += StringProcessing.BlankSpace / 2;
            }
            else if (temp1 is Chord chord1)
            {
                chord1.Chords.Last().Span += StringProcessing.BlankSpace / 2;
            }
            else if (temp1 is NullNote nunote1)
            {
                nunote1.Span += StringProcessing.BlankSpace / 2;
            }
            CurrentSong.Position = CurrentSong.Position;
        }
        private void Button_Click_7(object sender, RoutedEventArgs e)//下一个音符
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            IsPreviewSingleOne = true;
            CurrentSong.Position++;
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)//上一个音符
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            CurrentSong.Position--;
        }
        private void Close(object sender, RoutedEventArgs e)//退出专注
        {
            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.AccelerationRatio = 1;
            widthAnimation.From = 1440; // 起始宽度
            widthAnimation.To = 0;   // 结束宽度
            widthAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.4)); // 持续时间

            // 创建一个故事板，并将动画对象添加到其中
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);

            // 将动画应用到按钮的宽度属性
            Storyboard.SetTarget(widthAnimation, TimeEdit);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

            // 启动动画
            storyboard.Begin();
            IsAttentive = false;
        }
        #endregion


        #region 共用事件
        private void MouEnter(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.Cyan;
                Border? father = button.Parent as Border;
                if (father != null)
                {
                    father.BorderBrush = Brushes.Cyan;
                }
            }
        }
        private void MouLeave(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.Foreground = Brushes.White;
                Border? father = button.Parent as Border;
                if (father != null)
                {
                    father.BorderBrush = Brushes.White;
                }
            }
        }
        #endregion
    }
}
