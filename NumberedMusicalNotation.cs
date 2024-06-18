using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

/// <summary>
/// 枚举Core的基本属性
/// </summary>
public enum CoreSets
{
    Key,
    Type,
    IsBlankStay,
    IsFastColumn,
}

/// <summary>
/// 枚举Paragraph的基本属性
/// </summary>
public enum ParagraphSets
{
    Max_VerticalCore,
    IndexInMusicScore
}

/// <summary>
/// 枚举MusicScore的基本属性
/// </summary>
public enum MusicScoreSets
{
    Paragraphs,
    Index
}

namespace AutoPiano
{
    public class NumberedMusicalNotation : AudioBasic
    {
        #region 构成可视化简谱的核心四类
        /// <summary>
        /// 音符
        /// </summary>
        public class Core : StackPanel
        {
            public Core()
            {
                bt2.PreviewMouseLeftButtonDown += ChangeToHeight;
                bt2.PreviewMouseRightButtonDown += ChangeToCenter;

                bt3.PreviewMouseLeftButtonDown += ClearThisCore;
                bt3.PreviewMouseRightButtonDown += Start;
                bt3.PreviewMouseWheel += ChangeKey;


                bt4.PreviewMouseLeftButtonDown += ChangeTypeToLow;
                bt4.PreviewMouseRightButtonDown += ChangeTypeToTop;

                bt5.PreviewMouseLeftButtonDown += ChangeToLow;
                bt5.PreviewMouseRightButtonDown += ChangeToCenter;
                Children.Add(bt2);
                Children.Add(bt3);
                Children.Add(bt4);
                Children.Add(bt5);
            }

            private int _key = 1;

            private int _type = 1;

            /// <summary>
            /// 是否为占位符
            /// </summary>
            public bool IsBlankStay = false;

            /// <summary>
            /// 是否为滑音
            /// </summary>
            public bool IsFastColumn = false;

            /// <summary>
            /// 绝对时值
            /// </summary>
            public int TimeValue
            {
                get
                {
                    return GetTimeValueByType(this);
                }
            }

            /// <summary>
            /// 音阶
            /// </summary>
            public int Key
            {
                get
                {
                    return _key;
                }
                set
                {
                    if (value > -8 && value < 15 && value != 0)
                    {
                        _key = value;
                        return;
                    }
                    _key = 1;
                }
            }

            /// <summary>
            /// 时值类型
            /// </summary>
            public int Type
            {
                get { return _type; }
                set
                {
                    if (NoteTypes.Contains(value))
                    {
                        _type = value;
                        return;
                    }
                    _type = 1;
                }
            }

            /// <summary>
            /// 值设置
            /// </summary>
            /// <param name="set">int属性枚举</param>
            /// <param name="value">新值</param>
            public void Set(CoreSets set, int value)
            {
                switch (set)
                {
                    case CoreSets.Key: Key = value; break;
                    case CoreSets.Type: Type = value; break;
                }
            }

            /// <summary>
            /// 值设置
            /// </summary>
            /// <param name="set">bool属性枚举</param>
            /// <param name="value">新值</param>
            public void Set(CoreSets set, bool value)
            {
                switch (set)
                {
                    case CoreSets.IsBlankStay: IsBlankStay = value; break;
                    case CoreSets.IsFastColumn: IsFastColumn = value; break;
                }
            }

            /// <summary>
            /// 高音
            /// </summary>
            ButtonX bt2 = new ButtonX()
            {
                Height = 15,
                Width = 17,
                ButtonTextColor = Brushes.White,
                ButtonTextSize = 13,
                BorderAnimationSide = new Thickness(0),
                HoverTextColor = Brushes.Cyan
            };

            /// <summary>
            /// 音阶（不显示高低）
            /// </summary>
            ButtonX bt3 = new ButtonX()//
            {
                Height = 15,
                Width = 17,
                ButtonTextColor = Brushes.White,
                ButtonTextSize = 13,
                BorderAnimationSide = new Thickness(0),
                HoverTextColor = Brushes.Cyan
            };

            /// <summary>
            /// 时值类型
            /// </summary>
            ButtonX bt4 = new ButtonX()
            {
                Height = 15,
                Width = 17,
                ButtonTextColor = Brushes.White,
                ButtonTextSize = 13,
                BorderAnimationSide = new Thickness(0),
                HoverTextColor = Brushes.Cyan
            };

            /// <summary>
            /// 低音
            /// </summary>
            ButtonX bt5 = new ButtonX()
            {
                Height = 15,
                Width = 17,
                ButtonTextColor = Brushes.White,
                ButtonTextSize = 13,
                BorderAnimationSide = new Thickness(0),
                HoverTextColor = Brushes.Cyan
            };

            /// <summary>
            /// 从父级容器中移除此Core
            /// </summary>
            private void ClearThisCore(object sender, RoutedEventArgs e)
            {
                MessageBoxResult result = MessageBox.Show($"确定要删除这个音符吗?", "⚠危险操作", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // 获取父级 StackPanel
                    StackPanel parentStackPanel = Parent as StackPanel;

                    // 确保父级 StackPanel 不为空且确实包含该 Core
                    if (parentStackPanel != null && parentStackPanel.Children.Contains(this))
                    {
                        // 从父级 StackPanel 中移除该 Core
                        parentStackPanel.Children.Remove(this);
                    }
                }
            }

            /// <summary>
            /// 预览音符
            /// </summary>
            private void Start(object sender, RoutedEventArgs e)
            {
                Play_NoAuto();
            }

            /// <summary>
            /// 滚轮统计值
            /// </summary>
            public static int ScrollValue = 0;

            /// <summary>
            /// 改变音阶
            /// </summary>
            private void ChangeKey(object sender, MouseWheelEventArgs e)
            {
                if (IsBlankStay) { return; }
                ScrollValue += e.Delta;
                if (ScrollValue > 200)
                {
                    ScrollValue = 0;
                    if (Key > 0)
                    {
                        Key += 1;
                        ReLoadCore();
                        return;
                    }
                    Key -= 1;
                    ReLoadCore();
                }
                else if (ScrollValue < -200)
                {
                    ScrollValue = 0;
                    if (Key > 0)
                    {
                        if (_key - 1 == 0)
                        {
                            Key = -7;
                            ReLoadCore();
                            return;
                        }
                        Key -= 1;
                        return;
                    }
                    if (_key + 1 == 0)
                    {
                        Key = _key + 2;
                        ReLoadCore();
                        return;
                    }
                    Key += 1;
                }
                ReLoadCore();
                e.Handled = true;
            }

            /// <summary>
            /// 转为更短的时值类型
            /// </summary>
            private void ChangeTypeToLow(object sender, RoutedEventArgs e)
            {
                Type = _type / 2;
                ReLoadCore();
            }

            /// <summary>
            /// 转为更长的时值类型
            /// </summary>
            private void ChangeTypeToTop(object sender, RoutedEventArgs e)
            {
                Type = _type * 2;
                ReLoadCore();
            }

            /// <summary>
            /// 转为高音
            /// </summary>
            private void ChangeToHeight(object sender, RoutedEventArgs e)
            {
                if (IsBlankStay) { return; }
                if (Key > -8 && Key < 0)
                {
                    Key = LowToTop[_key];
                }
                else if (Key > 0 && Key < 8)
                {
                    Key = CenterToTop[_key];
                }
                ReLoadCore();
            }

            /// <summary>
            /// 转为中音
            /// </summary>
            private void ChangeToCenter(object sender, RoutedEventArgs e)
            {
                if (IsBlankStay) { return; }
                if (Key > 7 && Key < 15)
                {
                    Key = _key - 7;
                }
                else if (Key > -8 && Key < 0)
                {
                    Key = Math.Abs(_key);
                }
                ReLoadCore();
            }

            /// <summary>
            /// 转为低音
            /// </summary>
            private void ChangeToLow(object sender, RoutedEventArgs e)
            {
                if (IsBlankStay) { return; }
                if (Key > 7 && Key < 15)
                {
                    Key = TopToLow[_key];
                }
                else if (Key > 0 && Key < 8)
                {
                    Key = CenterToLow[_key];
                }
                ReLoadCore();
            }

            /// <summary>
            /// 返回此音符，注意：这将设置此Core的大小并计算一次子元素的状态信息
            /// </summary>
            /// <returns>Core</returns>
            public Core GetGrid()
            {
                //父级容器的信息
                Width = double.NaN;
                Height = 60;
                Background = Brushes.Transparent;
                Orientation = Orientation.Vertical;

                //计算一次子元素信息
                CalculateCoreInfo();

                return this;
            }

            /// <summary>
            /// 返回全新的默认音符
            /// </summary>
            /// <returns>Core</returns>
            public Core GetDefaultGrid()
            {
                Core core = new Core();
                core.Set(CoreSets.Key, 1);
                core.Set(CoreSets.Type, 16);
                return core.GetGrid();
            }

            /// <summary>
            /// 刷新音符状态
            /// </summary>
            public void ReLoadCore()
            {
                CalculateCoreInfo();
            }

            /// <summary>
            /// 计算音符子元素的状态信息
            /// </summary>
            public void CalculateCoreInfo()
            {
                bt2.ButtonText = string.Empty;
                bt3.ButtonText = string.Empty;
                bt4.ButtonText = string.Empty;
                bt5.ButtonText = string.Empty;

                //高低音
                if (!IsBlankStay)
                {
                    if (Key > -8 && Key < 0)
                    {
                        bt5.ButtonText = "•";
                        bt2.ButtonText = string.Empty;
                    }
                    else if (Key > 0 && Key < 8)
                    {
                        bt2.ButtonText = string.Empty;
                        bt5.ButtonText = string.Empty;
                    }
                    else
                    {
                        bt2.ButtonText = "•";
                        bt5.ButtonText = string.Empty;
                    }
                }

                //音阶 盒子宽度 时值类型
                switch (Type)
                {
                    case 1:
                        ReSetButtonsWidth(272);
                        if (IsBlankStay)
                        {
                            bt3.ButtonText = "0" + "                 ——        ——        ——  ";
                        }
                        else
                        {
                            bt3.ButtonText = IntToCoreString[Key] + "                 ——        ——        ——  ";
                        }
                        bt2.ButtonText += "                                                         ";
                        bt5.ButtonText += "                                                         ";
                        break;
                    case 2:
                        ReSetButtonsWidth(136);
                        if (IsBlankStay)
                        {
                            bt3.ButtonText = "0" + "             ——      ";
                        }
                        else
                        {
                            bt3.ButtonText = IntToCoreString[Key] + "             ——      ";
                        }
                        bt2.ButtonText += "                          ";
                        bt5.ButtonText += "                          ";
                        break;
                    case 4:
                        ReSetButtonsWidth(68);
                        if (IsBlankStay)
                        {
                            bt3.ButtonText = "0" + "            ";
                        }
                        else
                        {
                            bt3.ButtonText = IntToCoreString[Key] + "            ";
                        }
                        bt2.ButtonText += "            ";
                        bt5.ButtonText += "            ";

                        break;
                    case 8:
                        ReSetButtonsWidth(34);
                        if (IsBlankStay)
                        {
                            bt3.ButtonText = "0" + "    ";
                        }
                        else
                        {
                            bt3.ButtonText = IntToCoreString[Key] + "    ";
                        }
                        bt4.ButtonText = "——   ";
                        bt2.ButtonText += "    ";
                        bt5.ButtonText += "    ";
                        break;
                    case 16:
                        ReSetButtonsWidth(17);
                        if (IsBlankStay)
                        {
                            bt3.ButtonText = "0";
                        }
                        else
                        {
                            bt3.ButtonText = IntToCoreString[Key];
                        }
                        bt4.ButtonText += "═";
                        break;
                }
            }

            /// <summary>
            /// 返回一个全新的占位符
            /// </summary>
            /// <returns>Core</returns>
            public static Core BlankStayKey()
            {
                Core core = new Core();

                core.Set(CoreSets.IsBlankStay, true);
                core.Set(CoreSets.Type, 16);
                core.bt3.ButtonText = "0";
                core.bt4.ButtonText = "═";
                core.CalculateCoreInfo();

                return core;
            }

            /// <summary>
            /// 设置所有子元素的宽度
            /// </summary>
            /// <param name="value">宽度值</param>
            public void ReSetButtonsWidth(int value)
            {
                Width = value;
                bt2.Width = value;
                bt3.Width = value;
                bt4.Width = value;
                bt5.Width = value;
            }

            /// <summary>
            /// 当此音符开始播放,子元素背景色变红
            /// </summary>
            public void WhileStartThisCore()
            {
                bt2.Background = Brushes.Tomato;
                bt3.Background = Brushes.Tomato;
                bt4.Background = Brushes.Tomato;
                bt5.Background = Brushes.Tomato;
            }

            /// <summary>
            /// 当此音符播放结束,子元素背景恢复成白色
            /// </summary>
            public void WhileStopThisCore()
            {
                bt2.Background = Brushes.Transparent;
                bt3.Background = Brushes.Transparent;
                bt4.Background = Brushes.Transparent;
                bt5.Background = Brushes.Transparent;
            }

            /// <summary>
            /// 预览
            /// </summary>
            public async void Play_NoAuto()
            {
                WhileStartThisCore();
                if (IsBlankStay)
                {

                }
                else
                {
                    PlayWithInt(Key);
                }
                await Task.Delay(TypeToValue[Type]);
                WhileStopThisCore();
            }

            /// <summary>
            /// 游戏内展示琴谱
            /// </summary>
            public async void StartInGame()
            {
                WhileStartThisCore();
                await Task.Delay(TypeToValue[Type]);
                WhileStopThisCore();
            }

            /// <summary>
            /// 创建镜像
            /// </summary>
            /// <returns>Core</returns>
            public Core CreatCopyedOne()
            {
                Core core = new Core();
                core.Set(CoreSets.Key, Key);
                core.Set(CoreSets.Type, Type);
                core.Set(CoreSets.IsBlankStay, IsBlankStay);
                core.Set(CoreSets.IsFastColumn, IsFastColumn);

                return core.GetGrid();
            }
        }

        /// <summary>
        /// 音轨
        /// </summary>
        public class Track : StackPanel
        {
            public Track()
            {
                Orientation = Orientation.Horizontal;
                PlayTrack.PreviewMouseLeftButtonDown += ClearThisTrack;
                PlayTrack.PreviewMouseRightButtonDown += StartTrack;
                PlayTrack.PreviewMouseWheel += AddCores;
                Background = Brushes.Transparent;
                Width = Paragraph.Max_SixteenCore * 17 + 20;
                Height = double.NaN; // 设置高度为自动
                PlayTrack.MouseEnter += (sender, e) =>
                {
                    PlayTrack.Background = Brushes.Red;
                };
                PlayTrack.MouseLeave += (sender, e) =>
                {
                    PlayTrack.Background = Brushes.Lime;
                };
            }
            /// <summary>
            /// 播放单音轨
            /// </summary>
            Button PlayTrack = new Button()
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Cyan,
                Width = 18,
                Background = Brushes.Lime,
                Height = double.NaN,
            };

            /// <summary>
            /// 存储Core
            /// </summary>
            public StackPanel Cores = new StackPanel()
            {
                Width = Paragraph.Max_SixteenCore * 17,
                Background = Brushes.Transparent,
                Orientation = Orientation.Horizontal,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            /// <summary>
            /// 增加一个音符【1】或【0】
            /// </summary>
            private void AddCores(object sender, MouseWheelEventArgs e)
            {
                bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                if (isCtrlPressed)
                {
                    if (e.Delta < 0)
                    {
                        if (Cores.Children.Count > 1)
                        {
                            Cores.Children.RemoveAt(Cores.Children.Count - 1);
                        }
                    }
                    else
                    {
                        bool isNextCoreOver = IsNewCoreOverTrack();
                        if (isNextCoreOver)
                        {
                            MessageBox.Show("再添加就溢出啦！");
                            e.Handled = true;
                            return;
                        }
                        Cores.Children.Add(Core.BlankStayKey());
                    }
                }
                else
                {
                    if (e.Delta < 0)
                    {
                        if (Cores.Children.Count > 1)
                        {
                            Cores.Children.RemoveAt(Cores.Children.Count - 1);
                        }
                    }
                    else
                    {
                        bool isNextCoreOver = IsNewCoreOverTrack();
                        if (isNextCoreOver)
                        {
                            MessageBox.Show("再添加就溢出啦！");
                            e.Handled = true;
                            return;
                        }
                        Cores.Children.Add(new Core().GetDefaultGrid());
                    }
                }
                e.Handled = true;
            }

            /// <summary>
            /// 从父级容器中移除此音轨
            /// </summary>
            private void ClearThisTrack(object sender, RoutedEventArgs e)
            {
                MessageBoxResult result = MessageBox.Show($"确定要删除此音轨吗？", "⚠危险操作", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // 获取父级 StackPanel
                    StackPanel parentStackPanel = Parent as StackPanel;

                    // 确保父级 StackPanel 不为空且确实包含该 Core
                    if (parentStackPanel != null && parentStackPanel.Children.Contains(this))
                    {
                        // 从父级 StackPanel 中移除该 Core
                        parentStackPanel.Children.Remove(this);
                    }
                }
            }

            /// <summary>
            /// 音轨独奏
            /// </summary>
            private async void StartTrack(object sender, RoutedEventArgs e)
            {
                foreach (var item in Cores.Children)
                {
                    if (item is Core)
                    {
                        Core core = (Core)item;
                        core.Play_NoAuto();
                        await Task.Delay(core.TimeValue);
                    }
                }
            }

            /// <summary>
            /// 音轨独奏
            /// </summary>
            public async void StartTrack()
            {
                foreach (var item in Cores.Children)
                {
                    if (item is Core)
                    {
                        Core core = (Core)item;
                        core.Play_NoAuto();
                        await Task.Delay(core.TimeValue);
                    }
                }
            }

            /// <summary>
            /// 游戏内显示
            /// </summary>
            public async void StartInGame()
            {
                foreach (var item in Cores.Children)
                {
                    if (item is Core)
                    {
                        Core core = (Core)item;
                        core.StartInGame();
                        await Task.Delay(TypeToValue[core.Type]);
                    }
                }
            }

            /// <summary>
            /// 返回此音轨
            /// </summary>
            /// <returns>Track</returns>
            public Track GetGrid()
            {
                Children.Add(Cores);
                Children.Add(PlayTrack);
                return this;
            }

            /// <summary>
            /// 返回此音轨（无按钮版）
            /// </summary>
            /// <returns>Track</returns>
            public Track GetGridWithOutButton()
            {
                Children.Add(Cores);
                return this;
            }

            /// <summary>
            /// 返回一个新的默认音轨
            /// </summary>
            /// <returns>Track</returns>
            public Track GetDefaultGrid()
            {
                Track track = new Track();
                Core core = new Core();
                track.Cores.Children.Add(core.GetDefaultGrid());
                return track.GetGrid();
            }

            /// <summary>
            /// 拆解音轨数据
            /// </summary>
            /// <returns></returns>
            public int[] GetTrackData()
            {

                int[] result = new int[16 / RightNum * LeftNum];

                int Counter = -1;
                foreach (var item in Cores.Children)
                {
                    if (item is Core)
                    {
                        Core core = (Core)item;
                        if (core.IsBlankStay)
                        {
                            result[++Counter] = 0;
                            for (int i = 0; i < 16 / core.Type - 1; i++)
                            {
                                result[++Counter] = 0;
                            }
                        }
                        else
                        {
                            result[++Counter] = core.Key;
                            for (int i = 0; i < 16 / core.Type - 1; i++)
                            {
                                result[++Counter] = 0;
                            }
                        }
                    }
                }
                return result;
            }

            /// <summary>
            /// 排查音轨格式
            /// </summary>
            public bool IsTrackPrepared()
            {
                int CorrectLength = 16 / RightNum * LeftNum;//等价于多少个16分音符
                int RealLength = 0;//实际个数
                foreach (Core core in Cores.Children)
                {
                    RealLength += 16 / core.Type;
                }
                if (CorrectLength == RealLength)
                {
                    foreach (var core in Cores.Children)//没有问题转全白
                    {
                        if (core is Core)
                        {
                            Core core1 = (Core)core;
                            foreach (var button in core1.Children)
                            {
                                if (button is Button)
                                {
                                    Button button1 = (Button)button;
                                    button1.Background = Brushes.Transparent;
                                }
                            }
                        }
                    }
                    return true;
                }
                foreach (var core in Cores.Children)//问题音轨全红
                {
                    if (core is Core)
                    {
                        Core core1 = (Core)core;
                        foreach (var button in core1.Children)
                        {
                            if (button is Button)
                            {
                                Button button1 = (Button)button;
                                button1.Background = Brushes.Tomato;
                            }
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// 检查是否超出音轨长度
            /// </summary>
            /// <returns>True超出/False未超出</returns>
            public bool IsNewCoreOverTrack()
            {
                int CorrectLength = 16 / RightNum * LeftNum;//等价16分音符总数
                int RealLength = 0;//实际个数
                foreach (Core core in Cores.Children)
                {
                    RealLength += 16 / core.Type;
                }
                if (CorrectLength < RealLength + 1)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 创建镜像
            /// </summary>
            /// <returns>Track</returns>
            public Track CreatCopyedOne()
            {
                Track track = new Track();
                foreach (Core core in Cores.Children)
                {
                    track.Cores.Children.Add(core.CreatCopyedOne());
                }
                return track.GetGrid();
            }

            /// <summary>
            /// 创建无按钮的镜像
            /// </summary>
            /// <returns>Track</returns>
            public Track CreatCopyedOneWithOutButton()
            {
                Track track = new Track();
                track.Width = double.NaN;
                if (track.Children.Contains(track.PlayTrack))
                {
                    track.Children.Remove(track.PlayTrack);
                }
                foreach (Core core in Cores.Children)
                {
                    track.Cores.Children.Add(core.CreatCopyedOne());
                }
                return track.GetGridWithOutButton();
            }
        }

        /// <summary>
        /// 段落
        /// </summary>
        public class Paragraph : StackPanel
        {
            /// <summary>
            /// 存储小节的父容器
            /// </summary>
            public MusicScore? FatherBox;

            public Paragraph()
            {
                Orientation = Orientation.Horizontal;
                Width = double.NaN;
                Background = Brushes.Transparent;
                Height = double.NaN;
                PlayParagraph.PreviewMouseRightButtonDown += StartAllTracks;
                PlayParagraph.PreviewMouseLeftButtonDown += DispatcherAddTracks;
                PlayParagraph.MouseEnter += (sender, e) =>
                {
                    PlayParagraph.Background = Brushes.Red;
                };

                PlayParagraph.MouseLeave += (sender, e) =>
                {
                    PlayParagraph.Background = Brushes.Cyan;
                };
            }

            /// <summary>
            /// 小节独奏
            /// </summary>
            public Button PlayParagraph = new Button()
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Cyan,
                Width = 18,
                Background = Brushes.Cyan,
                Height = double.NaN,
            };

            /// <summary>
            /// 存储音轨
            /// </summary>
            public StackPanel Tracks = new StackPanel()
            {
                Width = double.NaN,
                Orientation = Orientation.Vertical,
                Background = Brushes.Transparent,
                Height = double.NaN,
            };

            /// <summary>
            /// 小节在简谱中的索引
            /// </summary>
            public int IndexInMusicScore = 1;

            /// <summary>
            /// 小节纵向的最大音符个数
            /// </summary>
            private int _maxvertical = 1;

            /// <summary>
            /// 小节横向最大16分音符个数
            /// </summary>
            public static int Max_SixteenCore { get { return 16 / RightNum * LeftNum; } }

            /// <summary>
            /// 访问小节纵向最大音符个数
            /// </summary>
            public int Max_VerticalCore
            {
                get { return _maxvertical; }
                set
                {
                    if (value > 1)
                    {
                        _maxvertical = value;
                        return;
                    }
                    _maxvertical = 1;
                }
            }

            /// <summary>
            /// 设置属性值
            /// </summary>
            /// <param name="set">属性枚举</param>
            /// <param name="value">新值</param>
            public void Set(ParagraphSets set, int value)
            {
                switch (set)
                {
                    case ParagraphSets.Max_VerticalCore: Max_VerticalCore = value; break;
                    case ParagraphSets.IndexInMusicScore: IndexInMusicScore = value; break;
                }
            }

            /// <summary>
            /// 音轨的【添加】或【复制】
            /// </summary>
            private void DispatcherAddTracks(object sender, RoutedEventArgs e)
            {
                bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                if (isCtrlPressed)
                {
                    if (FatherBox != null)
                    {
                        CopyThisToFahter();
                    }
                }
                else
                {
                    AddTrack(new Track().GetDefaultGrid());
                }
            }

            /// <summary>
            /// 小节的【播放】或【移除】
            /// </summary>
            private void StartAllTracks(object sender, RoutedEventArgs e)
            {
                bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                if (isCtrlPressed)
                {
                    MessageBoxResult result = MessageBox.Show($"确定要删除这个小节吗？", "⚠危险操作", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (FatherBox != null)
                        {
                            if (FatherBox.Paragraphs.Contains(this))
                            {
                                FatherBox.Paragraphs.Remove(this);
                            }
                            StackPanel stackPanel = Parent as StackPanel;
                            if (stackPanel != null)
                            {
                                if (stackPanel.Children.Contains(this))
                                {
                                    stackPanel.Children.Remove(this);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (IsParagraphPrepared())
                    {
                        foreach (var track in Tracks.Children)
                        {
                            if (track is Track)
                            {
                                Track track1 = (Track)track;
                                track1.StartTrack();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("⚠存在格式错误的音轨，已标记");
                    }
                }
            }

            /// <summary>
            /// 返回此小节
            /// </summary>
            /// <returns>Paragraph</returns>
            public Paragraph GetGrid()
            {
                Children.Add(PlayParagraph);
                Children.Add(Tracks);
                return this;
            }

            /// <summary>
            /// 生成一个默认小节
            /// </summary>
            /// <returns>Paragraph</returns>
            public Paragraph GetDefaultGrid()
            {

                Track track = new Track();
                Tracks.Children.Add(track.GetDefaultGrid());
                return GetGrid();
            }

            /// <summary>
            /// 添加音轨
            /// </summary>
            /// <param name="value">音轨实例</param>
            public void AddTrack(Track value)
            {
                Tracks.Children.Add(new Track().GetDefaultGrid());
            }

            public List<object> Sum_Tracks()//音轨叠加
            {
                List<object> result = new List<object>();//存放Chord和Note对象

                int TracksCount = Tracks.Children.Count;//该小节总计音轨数量  数组列数
                int TracksLength = 16 / RightNum * LeftNum;//16分音符总数     数组每列的长度
                int[,] analize = new int[TracksCount, TracksLength];//合成音轨所必须的分析结构

                //填入所有音轨的数据合集
                int count = 0;//起始列
                foreach (var track in Tracks.Children)
                {
                    if (track is Track)
                    {
                        Track track1 = (Track)track;
                        int[] ints = track1.GetTrackData();
                        for (int i = 0; i < ints.Length; i++)
                        {
                            analize[count, i] = ints[i];
                        }
                        count++;
                    }
                }
                //分析此二维数组
                for (int i = 0; i < TracksLength; i++)
                {
                    List<int> ints = new List<int>();//每个音轨的所有操作
                    for (int j = 0; j < TracksCount; j++)
                    {
                        ints.Add(analize[j, i]);
                    }
                    int Operate = 0;
                    foreach (int item in ints)
                    {
                        if (item != 0)
                        {
                            Operate++;
                        }
                    }
                    if (Operate == 0)
                    {
                        result.Add(new NullNote(SixteenthNote));
                    }
                    else if (Operate == 1)
                    {
                        foreach (int item in ints)
                        {
                            if (item != 0)
                            {
                                result.Add(new Note(IntToKeyCode[item], SixteenthNote));
                                break;
                            }
                        }
                    }
                    else
                    {
                        Chord chord = new Chord();
                        for (int m = 0; m < ints.Count; m++)
                        {
                            if (ints[m] != 0)
                            {
                                Note nt = new Note();
                                nt.Key = IntToKeyCode[ints[m]];
                                nt.Span = 0;
                                chord.Chords.Add(nt);
                            }
                        }
                        chord.NewSpan(SixteenthNote);
                        result.Add(chord);
                    }
                }

                //剔除多余的占位符
                RemoveNull(result, 0);
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i] is NullNote)
                    {
                        result.Remove(result[i]);
                        i--;
                    }
                }

                return result;
            }

            /// <summary>
            /// 递归,将部分占位符转化为时值
            /// </summary>
            /// <param name="Target">包含所有占位符在内的操作队列</param>
            /// <param name="StartIndex">清除空白符的起始索引</param>
            public void RemoveNull(List<object> Target, int StartIndex)
            {
                for (int j = StartIndex + 1; j < Target.Count; j++)
                {
                    if (Target[j] is NullNote)
                    {
                        if (Target[StartIndex] is Note)
                        {
                            Note note = (Note)Target[StartIndex];
                            NullNote not = (NullNote)Target[j];
                            note.Span += not.Span;
                        }
                        else if (Target[StartIndex] is Chord)
                        {
                            Chord note = (Chord)Target[StartIndex];
                            NullNote not = (NullNote)Target[j];
                            note.NewSpan(note.Chords.Last().Span + not.Span);
                        }
                        else if (Target[StartIndex] is NullNote)
                        {
                            NullNote note = (NullNote)Target[StartIndex];
                            NullNote not = (NullNote)Target[j];
                            note.Span += not.Span;
                        }
                    }
                    else
                    {
                        RemoveNull(Target, j);
                        break;
                    }
                }
            }

            /// <summary>
            /// 排查小节格式
            /// </summary>
            /// <returns>True格式正确/False格式错误</returns>
            public bool IsParagraphPrepared()
            {
                bool result = true;
                foreach (Track track in Tracks.Children)
                {
                    if (!track.IsTrackPrepared())
                    {
                        result = false;
                    }
                }
                return result;
            }

            /// <summary>
            /// 创建镜像
            /// </summary>
            /// <returns>Paragraph</returns>
            public Paragraph CreatCopyedOne()
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Children.Add(paragraph.PlayParagraph);
                paragraph.Children.Add(paragraph.Tracks);
                paragraph.FatherBox = FatherBox;
                foreach (Track track in Tracks.Children)
                {
                    paragraph.Tracks.Children.Add(track.CreatCopyedOne());
                }
                return paragraph;
            }

            /// <summary>
            /// 创建不带按钮的镜像
            /// </summary>
            /// <returns>Paragraph</returns>
            public Paragraph CreatCopyedOneWithOutButton()
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Children.Add(paragraph.Tracks);
                paragraph.FatherBox = FatherBox;
                paragraph.Width = double.NaN;
                foreach (Track track in Tracks.Children)
                {
                    paragraph.Tracks.Children.Add(track.CreatCopyedOneWithOutButton());
                }
                return paragraph;
            }

            /// <summary>
            /// 在父级容器中添加一个此小节的镜像
            /// </summary>
            public void CopyThisToFahter()
            {
                StackPanel parentStackPanel = Parent as StackPanel;

                if (parentStackPanel != null)
                {
                    Paragraph paragraph = CreatCopyedOne();
                    parentStackPanel.Children.Add(paragraph);
                    FatherBox.Paragraphs.Add(paragraph);
                }
            }
        }

        /// <summary>
        /// 简谱
        /// </summary>
        public class MusicScore : StackPanel
        {
            private bool _isOn = false;
            public bool IsPlaying
            {
                get { return _isOn; }
                set { _isOn = value; }
            }
            private bool IfOver = false;
            public MusicScore()
            {
                Orientation = Orientation.Horizontal;
            }

            private List<Paragraph> _paragraphs = new List<Paragraph>();
            /// <summary>
            /// 存储Paragraph
            /// </summary>
            public List<Paragraph> Paragraphs
            {
                get { return _paragraphs; }
                set
                {
                    _paragraphs = value;
                    LoadUI();
                }
            }

            public int Count
            {
                get { return Paragraphs.Count; }
            }

            Stopwatch stopwatch = new Stopwatch();//计算循环所耗费的时间

            private void LoadUI()
            {
                Children.Clear();
                foreach (Paragraph data in Paragraphs)
                {
                    Children.Add(data);
                    data.FatherBox = this;
                }
            }

            public void ParseMetaData(MetaData metaData)
            {
                Paragraphs.Clear();
                foreach (Paragraph data in metaData.GetMusicScore().Paragraphs)
                {
                    Paragraphs.Add(data);
                }
                LoadUI();
            }

            public void SetValue(MusicScoreSets sets, int index, Paragraph value)
            {
                switch (sets)
                {
                    case MusicScoreSets.Paragraphs: Paragraphs[index] = value; break;
                }
            }

            public async void Start()//全局预览
            {
                if (IsWorking && !IfOver)
                {
                    return;
                }
                IsWorking = true;
                foreach (Paragraph paragraph in Paragraphs)
                {
                    if (IfOver)
                    {
                        IsWorking = false;
                        IfOver = false;
                        return;
                    }
                    if (IsWorkStop)
                    {
                        IsWorkStop = false;
                        return;
                    }

                    stopwatch.Reset();
                    stopwatch.Start();
                    foreach (var item in paragraph.Children)
                    {
                        if (item is StackPanel)
                        {
                            StackPanel stackPanel = (StackPanel)item;
                            foreach (var obj in stackPanel.Children)
                            {
                                if (obj is Track)
                                {
                                    Track track = (Track)obj;
                                    track.StartTrack();
                                }
                            }
                        }
                    }
                    stopwatch.Stop();
                    await Task.Delay(BasicValue - (int)stopwatch.ElapsedMilliseconds);
                }
                IsWorking = false;
            }
            public void Stop()
            {
                if (IsWorking)
                {
                    IfOver = true;
                }
            }

            /// <summary>
            /// 【添加】小节对象至指定StackPanel容器中
            /// </summary>
            /// <param name="target">目标容器</param>
            public void AddDefaultParagraph()
            {
                Paragraph result = new Paragraph().GetDefaultGrid();
                result.IndexInMusicScore = Children.Count;
                result.FatherBox = this;
                Paragraphs.Add(result);
                Children.Add(result);
            }

            /// <summary>
            /// ⚠危险操作    从末尾处删除一个小节
            /// </summary>
            /// <param name="target">目标容器</param>
            public void DeleteLastParagraph()
            {
                if (Paragraphs.Count > 0)
                {
                    Children.RemoveAt(Children.Count - 1);
                    Paragraphs.RemoveAt(Paragraphs.Count - 1);
                }
            }

            /// <summary>
            /// ⚠危险操作 将此简谱对象的全部数据【覆写】
            /// </summary>
            /// <param name="target">目标容器</param>
            public void Update()
            {
                Children.Clear();
                int Counter = 0;
                foreach (Paragraph paragraph in Paragraphs)
                {
                    paragraph.IndexInMusicScore = Counter;
                    Children.Add(paragraph);
                    Counter++;
                }
            }

            public Song ConvertToSong()
            {
                Song result = new Song();
                foreach (Paragraph paragraph in Paragraphs)
                {
                    foreach (var obj in paragraph.Sum_Tracks())
                    {
                        result.notes.Add(obj);
                    }
                }
                return result;
            }

            /// <summary>
            /// 根据元数据【增加】简谱工作簿
            /// </summary>
            /// <param name="metaData">增加的数据</param>
            public void AddCopyOneByMetaData(MetaData metaData)
            {
                foreach (Paragraph data in metaData.GetMusicScore().Paragraphs)
                {
                    Paragraphs.Add(data);
                    data.FatherBox = this;
                }
            }

            /// <summary>
            /// 排查乐谱格式
            /// </summary>
            public bool IsMusicPrepared()
            {
                bool result = true;
                foreach (Paragraph paragraph in Paragraphs)
                {
                    if (!paragraph.IsParagraphPrepared())
                    {
                        result = false;
                    }
                }
                return result;
            }
        }
        #endregion       
    }
}
