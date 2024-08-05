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
using System.Windows.Controls.Primitives;
using FastHotKeyForWPF;

namespace AutoPiano
{
    /// <summary>
    /// 文本乐谱的交互界面
    /// </summary>
    public partial class TxtAnalizeVisual : Page
    {
        public static TxtAnalizeVisual? Instance;

        public static double SpeedChangeRate = 0.1;

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

        public static PlayModel _currentPlayModel = PlayModel.Preview;
        public static PlayModel CurrentPlayModel//播放模式
        {
            get => _currentPlayModel;
            set
            {
                CurrentSong.Pause();
                _currentPlayModel = value;
                _data.Model = _currentPlayModel;
            }
        }

        public static bool PopupControl//弹窗开关
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.VisualBox.IsOpen;
                }
                return false;
            }
            set
            {
                if (Instance != null)
                {
                    if (value == Instance.VisualBox.IsOpen) { return; }
                    Instance.VisualBox.IsOpen = value;
                    CurrentPlayModel = value ? PlayModel.Auto : PlayModel.Preview;
                }
            }
        }

        private static Song? _data = new Song();
        /// <summary>
        /// 当前界面存放的曲目
        /// </summary>
        public static Song CurrentSong//当前播放的音乐
        {
            get { return _data == null ? new Song() : _data; }
            set
            {
                if (_data == null) { return; }
                _data.Pause();
                _data.IsDestroyed = true;
                _data = null;

                _data = value;
                if (Instance != null)
                {
                    Instance.Notes.Children.Clear();
                    foreach (StackPanel textBlock in Instance.LoadPanelBoxes())
                    {
                        Instance.Notes.Children.Add(textBlock);
                    }
                    _data.Model = CurrentPlayModel;
                    Instance.TimeValue.Text = string.Empty;
                    Instance.SongName.Text = _data.Name;
                    Instance.VisualInGame.SongInfo.Text = "《 " + _data.Name + " 》";

                    Instance.ProcessShow.SetValue(0);
                }
            }
        }

        public static bool IsAttentive = false;//是否处于专注

        public static bool IsPreviewSingleOne = false;//切换音符操作需要预览音符效果时，请设为true，生效一次

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

        public static Slider Slider//进度拖条
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
        public static PopupX PopupX//弹窗内部组件
        {
            get
            {
                if (Instance != null) { return Instance.VisualInGame; }
                return new PopupX();
            }
            set
            {
                if (Instance != null) { Instance.VisualInGame = value; }
            }
        }

        public static bool IsPageLoaded = false;//页面是否加载完成
        public static bool IsNormalOutput = true;//是否采用通用协议输出数据
        public static bool IsNormalInput = true;//是否采用通用协议读取数据

        public static DataTypes ReadModel
        {
            get => IsNormalInput ? DataTypes.PublicAutoData : DataTypes.PrivateAutoData;
        }

        public static DataTypes WriteModel
        {
            get => IsNormalOutput ? DataTypes.PublicAutoData : DataTypes.PrivateAutoData;
        }

        public TxtAnalizeVisual()
        {
            InitializeComponent();
            Instance = this;
            IsPageLoaded = true;
            VisualInGame.SongSelect = LoadFilesBox;
            ProcessShow.AfterSetValue += (progressX) =>
            {
                VisualInGame.UpdateProgress(ProcessShow.ProgressRate);
            };
        }

        public static void OpenPopup()
        {
            if (Instance != null)
            {
                Instance.VisualBox.IsOpen = true;
            }
        }
        public static void ClosePopup(object sender, RoutedEventArgs e)
        {
            if (Instance != null)
            {
                Instance.VisualBox.IsOpen = false;
            }
        }


        public void LoadFixedHotKey()//注册一些固定不可变的快捷键
        {
            GlobalHotKey.Add(ModelKeys.CTRL, NormalKeys.LEFT, LastNote);
            GlobalHotKey.Add(ModelKeys.CTRL, NormalKeys.RIGHT, NextNote);
            GlobalHotKey.Add(ModelKeys.CTRL, NormalKeys.UP, AddSpan);
            GlobalHotKey.Add(ModelKeys.CTRL, NormalKeys.DOWN, DeleteSpan);

            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.LEFT);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.RIGHT);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.UP);
            GlobalHotKey.ProtectHotKeyByKeys(ModelKeys.CTRL, NormalKeys.DOWN);
        }


        #region 拖条处理
        public bool IsMouseInSlider = false;
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsMouseInSlider && CurrentSong.IsOnPlaying) { CurrentSong.Pause(); IsMouseInSlider = false; }
            if (IsMouseInSlider) { CurrentSong.Position = (int)(e.NewValue * (CurrentSong.notes.Count - 1)); }
        }
        private void PSliderMouseEnter(object sender, MouseEventArgs e)
        {
            IsMouseInSlider = true;
        }
        private void PSliderMouseLeave(object sender, MouseEventArgs e)
        {
            IsMouseInSlider = false;
        }
        #endregion


        #region 文本解析器
        private void Button_Click(object sender, RoutedEventArgs e)//Txt解析
        {
            CurrentSong.Stop();

            try
            {
                var result = FileTool.ReadTxtFile();
                CurrentSong = StringProcessing.SongParse(result.Item3);
                SongName.Text = result.Item2;
            }
            catch { MessageBox.Show("解析txt失败，请检查文本格式是否正确!"); }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)//粘贴板解析
        {
            CurrentSong.Stop();

            try
            {
                if (IsNormalInput)
                {
                    CurrentSong = StringProcessing.NormalDataToSong(Clipboard.GetText());
                    SongName.Text = CurrentSong.Name;
                }
                else
                {
                    CurrentSong = StringProcessing.SongParse(Clipboard.GetText());
                    SongName.Text = "? ? ?";
                }
            }
            catch { MessageBox.Show("解析粘贴板失败，请检查文本格式是否正确！"); }
        }
        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)//一个空格所代表的时值
        {
            SpaceValue.Text = "当前 : " + (int)e.NewValue;
            StringProcessing.BlankSpace = (int)e.NewValue;
            if (IsPageLoaded) LittelChange.Value = e.NewValue / 8;
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

            if (IsNormalOutput)
            {
                if (FileTool.SerializeObject<Song>(CurrentSong, DataTypes.PublicAutoData, CurrentSong.Name))
                {
                    SaveButton.Content = "完成√";
                    await Task.Delay(2500);
                    SaveButton.Content = "存档";
                    SaveButton.Foreground = Brushes.White;
                    return;
                }
            }
            else
            {
                if (FileTool.SerializeObject<Song>(CurrentSong, DataTypes.PrivateAutoData, CurrentSong.Name))
                {
                    SaveButton.Content = "完成√";
                    await Task.Delay(2500);
                    SaveButton.Content = "存档";
                    SaveButton.Foreground = Brushes.White;
                    return;
                }
            }

            SaveButton.Foreground = Brushes.Red;
            SaveButton.Content = "⚠失败";
            await Task.Delay(2500);
            SaveButton.Content = "存档";
            SaveButton.Foreground = Brushes.White;
        }
        private async void Button_Click_3(object sender, RoutedEventArgs e)//读档
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); }
            ReadButton.Content = "读取ing……";
            ReadButton.Foreground = Brushes.Lime;

            if (IsNormalInput)
            {
                var result = FileTool.DeserializeObject<Song>(DataTypes.PublicAutoData);
                if (result.Item1)
                {
                    CurrentSong = result.Item2;
                    ReadButton.Foreground = Brushes.Red;
                    ReadButton.Content = "成功√";
                    await Task.Delay(2500);
                    ReadButton.Content = "读档";
                    ReadButton.Foreground = Brushes.White;
                    return;
                }
            }
            else
            {
                var result = FileTool.DeserializeObject<Song>(DataTypes.PrivateAutoData);
                if (result.Item1)
                {
                    CurrentSong = result.Item2;
                    ReadButton.Foreground = Brushes.Red;
                    ReadButton.Content = "成功√";
                    await Task.Delay(2500);
                    ReadButton.Content = "读档";
                    ReadButton.Foreground = Brushes.White;
                    return;
                }
            }

            ReadButton.Foreground = Brushes.Red;
            ReadButton.Content = "⚠失败";
            await Task.Delay(2500);
            ReadButton.Content = "读档";
            ReadButton.Foreground = Brushes.White;
        }
        #endregion


        #region 界面UI变动
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
            try
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
            catch { }
        }
        #endregion


        #region 时值编辑器
        private void AddNewParagraph(object sender, RoutedEventArgs e)//从当前位置替换一段新的解析结果
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Stop(); }

            if (IsNormalInput)
            {
                var result = FileTool.DeserializeObject<Song>(DataTypes.PublicAutoData).Item2;
                Song temp = Song.AddParagraph(CurrentSong, result);
                CurrentSong = temp;
            }
            else
            {
                var result = FileTool.ReadTxtFile();
                Song temp = Song.AddParagraph(CurrentSong, result.Item3);
                CurrentSong = temp;
            }
        }
        private void ClearNote(object sender, RoutedEventArgs e)//清除当前位置的音符
        {
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            Song temp = Song.AddParagraph(CurrentSong, " ");
            CurrentSong = temp;
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)//加速
        {
            DeleteSpan();
        }
        private void Button_Click_9(object sender, RoutedEventArgs e)//降速
        {
            AddSpan();
        }
        private void Button_Click_7(object sender, RoutedEventArgs e)//下一个音符
        {
            NextNote();
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)//上一个音符
        {
            LastNote();
        }
        private void Close(object sender, RoutedEventArgs e)//退出专注
        {
            UnExpendBox();
        }
        public void AddSpan(object sender, HotKeyEventArgs e)
        {
            AddSpan();
        }
        public void AddSpan()
        {
            if (EditArea.PageType != PageTypes.TxtAnalize) { return; }
            try
            {
                if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
                object temp1 = CurrentSong.notes[CurrentSong.Position];
                if (temp1 is Note note1)
                {
                    note1.Span += StringProcessing.BlankSpace_Re;
                }
                else if (temp1 is Chord chord1)
                {
                    chord1.Chords.Last().Span += StringProcessing.BlankSpace_Re;
                }
                else if (temp1 is NullNote nunote1)
                {
                    nunote1.Span += StringProcessing.BlankSpace_Re;
                }
                CurrentSong.Position = CurrentSong.Position;
            }
            catch { }
        }
        public void DeleteSpan(object sender, HotKeyEventArgs e)
        {
            DeleteSpan();
        }
        public void DeleteSpan()
        {
            if (EditArea.PageType != PageTypes.TxtAnalize) { return; }
            try
            {
                if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
                object temp = CurrentSong.notes[CurrentSong.Position];
                if (temp is Note note)
                {
                    int a = note.Span - StringProcessing.BlankSpace_Re;
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
                    int a = chord.Chords.Last().Span - StringProcessing.BlankSpace_Re;
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
                    int a = nunote.Span - StringProcessing.BlankSpace_Re;
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
            catch { }
        }
        public void NextNote(object sender, HotKeyEventArgs e)
        {
            NextNote();
        }
        public void NextNote()
        {
            if (EditArea.PageType != PageTypes.TxtAnalize) { return; }
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            IsPreviewSingleOne = true;
            CurrentSong.Position++;
        }
        public void LastNote(object sender, HotKeyEventArgs e)
        {
            LastNote();
        }
        public void LastNote()
        {
            if (EditArea.PageType != PageTypes.TxtAnalize) { return; }
            if (CurrentSong.IsOnPlaying) { CurrentSong.Pause(); return; }
            CurrentSong.Position--;
        }
        #endregion


        #region 共用事件
        private void MouEnter(object sender, RoutedEventArgs e)//进入变色
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
        private void MouLeave(object sender, RoutedEventArgs e)//离开变色
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
        public void RateFaster(object sender, RoutedEventArgs e)//比率加速
        {
            CurrentSong *= (1 - SpeedChangeRate);
        }
        public void RateSlower(object sender, RoutedEventArgs e)//比率降速
        {
            CurrentSong *= (1 + SpeedChangeRate);
        }
        #endregion


        #region 文件选择视窗
        public static void LoadFilesBox(object sender, RoutedEventArgs e)
        {
            if (Instance != null)
            {
                Instance.FileInfos.Children.Clear();

                var result = FileTool.GetFilesInfo(ReadModel);

                for (int i = 0; i < result.Item1.Count; i++)
                {
                    FileButton temp = new FileButton();
                    temp.FileName = result.Item1[i];
                    temp.Content = result.Item1[i];
                    temp.Foreground = Brushes.White;
                    temp.Background = Brushes.Transparent;
                    temp.FontSize = 26;
                    temp.FilePath = result.Item2[i];
                    Instance.FileInfos.Children.Add(temp);
                }

                Instance.FileSelectInGame.IsOpen = true;
            }
        }
        public static void CloseFilesBox()
        {
            if (Instance != null)
            {
                Instance.FileSelectInGame.IsOpen = false;
            }
        }
        public class FileButton : Button
        {
            public FileButton()
            {
                Width = 300;
                Height = 40;
                BorderThickness = new Thickness(0);
                Click += GuidToSong;
                MouseEnter += (sender, e) =>
                {
                    Foreground = Brushes.Red;
                };
                MouseLeave += (sender, e) =>
                {
                    Foreground = Brushes.White;
                };
            }
            public string FileName = "None";
            public string FilePath = "None";
            public void GuidToSong(object sender, RoutedEventArgs e)
            {
                var result = FileTool.DeserializeObject<Song>(ReadModel, FilePath);
                if (result.Item1 && result.Item2 != null)
                {
                    CurrentSong = result.Item2;
                    CurrentSong.Position = 0;
                }

                CloseFilesBox();
            }
        }
        #endregion
    }
}
