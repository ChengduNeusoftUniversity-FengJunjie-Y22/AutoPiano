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
    /// TxtAnalizeVisual.xaml 的交互逻辑
    /// </summary>
    public partial class TxtAnalizeVisual : Page
    {
        public static TxtAnalizeVisual? Instance;

        public static InstrumentTypes _instrumentType = InstrumentTypes.FWPiano;
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
        public static Song CurrentSong
        {
            get { return _data; }
            set
            {
                _data = value;
                if (Instance != null)
                {
                    Instance.SDValuePlay.Maximum = (CurrentSong.notes.Count + 12) * 60;
                    Instance.SongName.Text = _data.Name;
                    _data.Position = 0;
                    CheckPoint = _data.notes.Count - 14;
                    _data.IsOnPlaying = false;
                    _data.IsStop = false;
                    _data.Model = PlayModel.Preview;
                    Instance.TimeValue.Text = string.Empty;
                    Instance.Notes.Children.Clear();
                    foreach (StackPanel textBlock in Instance.LoadPanelBoxes())
                    {
                        Instance.Notes.Children.Add(textBlock);
                    }
                    MainWindow.AutoTarget = Song.Copy(CurrentSong);
                    MainWindow.AutoTarget.Model = PlayModel.Auto;
                }
            }
        }

        public static bool IsAttentive = false;

        public static int CheckPoint = 0;

        public static bool IsPreviewSingleOne = false;

        public TxtAnalizeVisual()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentSong.Position < CheckPoint)
            {
                TxtNotes.ScrollToHorizontalOffset(e.NewValue - 12 * 60);
            }
            else
            {
                TxtNotes.ScrollToHorizontalOffset(SDValuePlay.Maximum);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentSong.Stop();
            var result = await StringProcessing.SelectThenAnalize();
            CurrentSong = result.Item1;
            SongName.Text = result.Item2;
            SDValuePlay.Maximum = CurrentSong.notes.Count * 60;
        }

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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentSong.Stop();
            CurrentSong = await StringProcessing.SongParse(Clipboard.GetText());
            SongName.Text = "? ? ?";
        }

        private StackPanel[] LoadPanelBoxes()
        {
            StackPanel[] result = new StackPanel[CurrentSong.notes.Count];
            SDValuePlay.Maximum = CurrentSong.notes.Count * 60;
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

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SpaceValue.Text = "当前 : " + (int)e.NewValue;
            StringProcessing.BlankSpace = (int)e.NewValue;
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AdjustValue.Text = "当前 : " + (int)e.NewValue;
            StringProcessing.BlankSpace_Re = (int)e.NewValue;
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

        private void SDValuePlay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CurrentSong.Position = (int)(SDValuePlay.Value / 60 - 12);
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); }
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

        private async void Button_Click_3(object sender, RoutedEventArgs e)
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

        private void Button_Click_5(object sender, RoutedEventArgs e)
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

        public void NewInfo(object target)
        {
            AIndex.Text = CurrentSong.Position.ToString();
            double result = CurrentSong.Position * 60;
            SDValuePlay.Value = result;
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

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            CurrentSong.Position--;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            IsPreviewSingleOne = true;
            CurrentSong.Position++;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
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

        private void Button_Click_9(object sender, RoutedEventArgs e)
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

        private void Button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (key == Key.D0) { CurrentSong.Position = 0; return; }

            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            IsPreviewSingleOne = true;
            CurrentSong.Position++;

            e.Handled = true;
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            KeyDArea.Focus();
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
