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
                    Instance.SongName.Text = _data.Name;
                    _data.Position = 0;
                    _data.IsOnPlaying = false;
                    _data.IsStop = false;
                    Instance.TimeValue.Text = string.Empty;
                    Instance.Notes.Children.Clear();
                    foreach (StackPanel textBlock in Instance.LoadPanelBoxes())
                    {
                        Instance.Notes.Children.Add(textBlock);
                    }
                }
            }
        }
        public TxtAnalizeVisual()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TxtNotes.ScrollToHorizontalOffset(e.NewValue - 12 * 60);
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
    }
}
