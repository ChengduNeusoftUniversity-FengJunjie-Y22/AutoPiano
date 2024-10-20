using Microsoft.Expression.Shapes;
using MinimalisticWPF;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace AutoPiano
{
    public static class DynamicVisionGroup
    {
        private static List<string> type0 = new List<string>() { "Q", "W", "E", "R", "T", "Y", "U" };
        private static List<string> type1 = new List<string>() { "A", "S", "D", "F", "G", "H", "J" };
        private static List<string> type2 = new List<string>() { "Z", "X", "C", "V", "B", "N", "M" };
        private static double labelsize = 50;
        public static double LabelSize
        {
            get => labelsize;
            set
            {
                labelsize = value;
                LT.Width = labelsize;
                LT.Height = labelsize;
                RB.Width = labelsize;
                RB.Height = labelsize;
            }
        }
        public static Popup Popup { get; set; } = new Popup();
        public static Canvas Canvas { get; set; } = new Canvas();
        public static Arc LT { get; set; } = new Arc()
        {
            Stretch = Stretch.None,
            EndAngle = 120,
            Width = LabelSize,
            Height = LabelSize,
            Fill = Brushes.Cyan,
            ArcThickness = 5,
        };
        public static Arc RB { get; set; } = new Arc()
        {
            Stretch = Stretch.None,
            EndAngle = 120,
            Width = LabelSize,
            Height = LabelSize,
            Fill = Brushes.Violet,
            ArcThickness = 5,
        };
        public static Tuple<Point2f, Point2f> KeyArea { get; set; } = Tuple.Create(new Point2f(0, 0), new Point2f(0, 0));
        public static Point2f[,] Positions { get; set; } = new Point2f[3, 7];
        public static int AnimationTime { get; set; } = 1000;
        public static Queue<Tuple<List<Tuple<int, List<TransitionBoard<Grid>>>>, int>> AnimationData { get; set; } = new Queue<Tuple<List<Tuple<int, List<TransitionBoard<Grid>>>>, int>>();
        public static void Parse(Song song)
        {
            //获取音符元素
            var targets = song.notes.OfType<IMusicMethod>().ToList();

            //计算音乐总时长
            var fulltime = targets.Sum(x => x.Span);

            //计算节点的起始时刻
            for (int i = 1; i < targets.Count; i++)
            {
                targets[i].StartTime = targets[i - 1].StartTime + targets[i - 1].Span;
            }
            AnimationData.Clear();
            IntervalScanning(targets, fulltime);
        }
        public static async Task Start()
        {
            foreach (var data in AnimationData)
            {
                foreach (var animation in data.Item1)
                {
                    await Task.Delay(animation.Item1);
                    foreach (var transition in animation.Item2)
                    {
                        transition.Start();
                    }
                }
                await Task.Delay(data.Item2);
            }
        }

        public static void IntervalScanning(List<IMusicMethod> Values, int fulltime, int IntervalIndex = 0)//区间扫描
        {
            //检测区间
            var min = IntervalIndex * AnimationTime;
            IntervalIndex++;
            var max = IntervalIndex * AnimationTime;

            if (min > fulltime)
            {
                return;
            }

            //区间内部元素
            var targets = Values.Where(x => x.StartTime >= min && x.StartTime < max).ToList();

            //有序等待时长
            var times = GetForeTime(targets, min);
            //距离下一个区间的等待时长 
            var endwait = GetEndTime(targets, max);
            //动画集合
            var transitions = GetTransitionBoard(targets);

            var data = SumTimeAndBoard(times, transitions);

            AnimationData.Enqueue(Tuple.Create(data, endwait));

            IntervalScanning(Values, fulltime, IntervalIndex);
        }
        public static List<int> GetForeTime(List<IMusicMethod> targets, int min)//预加载区间内有序等待时长
        {
            var result = new List<int>();
            for (int i = 0; i < targets.Count; i++)
            {
                var foretime = targets[i].StartTime - min;
                min += foretime;
                result.Add(foretime);
            }
            return result;
        }
        public static int GetEndTime(List<IMusicMethod> targets, int max)//预加载区间末尾等待
        {
            var result = targets.LastOrDefault();
            return result == null ? 0 : max - result.StartTime;
        }
        public static List<List<TransitionBoard<Grid>>> GetTransitionBoard(List<IMusicMethod> targets)//预加载动画效果
        {
            var count = 0;
            var result = new List<List<TransitionBoard<Grid>>>();
            foreach (var target in targets)
            {
                result.Add(new List<TransitionBoard<Grid>>());
                var points = target.GetStringNodes().SiteQuery();
                for (int k = 0; k < points.Count; k++)
                {
                    var block = new Grid()
                    {
                        Opacity = 0,
                        Height = labelsize / 2,
                        Width = labelsize,
                        Background = points[k].x == 0 ? Brushes.Cyan : points[k].x == 1 ? Brushes.Red : Brushes.Lime,
                    };
                    var point = Positions[points[k].x, points[k].y];
                    Canvas.SetTop(block, point.Y.DpiConvert() + labelsize / 2 - 500.DpiConvert());
                    Canvas.SetLeft(block, (point.X).DpiConvert() - labelsize / 2);
                    Canvas.Children.Add(block);
                    var delete = Transition.CreateBoardFromType<Grid>()
                        .SetProperty(x => x.Opacity, 0)
                        .SetProperty(x => x.Width, LabelSize * 1.7)
                        .SetProperty(x => x.Height, LabelSize * 1.7)
                        .SetParams((x) =>
                        {
                            x.Duration = 0.5;
                        });
                    var animator = block.Transition()
                        .SetProperty(x => x.RenderTransform, new TranslateTransform(0, 500))
                        .SetParams((x) =>
                        {
                            x.Duration = AnimationTime / 1000.0;
                            x.Start += () =>
                            {
                                block.Opacity = 1;
                            };
                            x.Completed += () =>
                            {
                                //结束时自行隐藏
                                block.BeginTransition(delete);
                                target.Preview();
                            };
                        })
                        .PreLoad();
                    result[count].Add(animator);
                }
                count++;
            }
            return result;
        }
        public static List<Tuple<int, List<TransitionBoard<Grid>>>> SumTimeAndBoard(List<int> times, List<List<TransitionBoard<Grid>>> transitions)
        {
            var result = new List<Tuple<int, List<TransitionBoard<Grid>>>>();

            var TIcount = times.Count;
            var TRcount = transitions.Count;

            if (TIcount == TRcount)
            {
                for (int i = 0; i < TIcount; i++)
                {
                    result.Add(Tuple.Create(times[i], transitions[i]));
                }
            }

            return result;
        }
        public static void SetPopupScreen()//初始化弹窗区域
        {
            Canvas.Children.Add(LT);
            Canvas.Children.Add(RB);
            Popup.Child = Canvas;
            Popup.AllowsTransparency = true;
            Popup.IsOpen = true;

            var AbsSize = CvTool.GetScreenSize();
            double newWidth = AbsSize.Item1.DpiConvert();
            double newHeight = AbsSize.Item2.DpiConvert();

            System.Windows.Rect rtWnd = new System.Windows.Rect(0, 0, newWidth, newHeight);
            DependencyObject parent = Popup.Child;
            do
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent != null && parent.ToString() == "System.Windows.Controls.Primitives.PopupRoot")
                {
                    var element = parent as FrameworkElement;
                    element.Width = newWidth;
                    element.Height = newHeight;
                    Popup.PlacementRectangle = rtWnd;
                    break;
                }
            }
            while (parent != null);

            LT.Transition()
                .SetProperty(x => x.StartAngle, 360)
                .SetProperty(x => x.EndAngle, 480)
                .SetParams((x) =>
                {
                    x.Duration = 0.5;
                    x.LoopTime = int.MaxValue;
                })
                .Start();
            RB.Transition()
                .SetProperty(x => x.StartAngle, 360)
                .SetProperty(x => x.EndAngle, 480)
                .SetParams((x) =>
                {
                    x.Duration = 0.5;
                    x.LoopTime = int.MaxValue;
                })
                .Start();
        }
        public static void UpdateArea()//更新按键区域
        {
            var result = CvTool.GetArea();
            var lt = result.Item1;
            var rb = result.Item2;
            KeyArea = Tuple.Create(lt, rb);
            var deltaX = (rb.X - lt.X) / 6;
            var deltaY = (lt.Y - rb.Y) / 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Positions[i, j] = new Point2f(lt.X + deltaX * j, lt.Y + deltaY * (2 - i));
                }
            }
            Canvas.SetLeft(LT, KeyArea.Item1.X.DpiConvert() - LabelSize / 2);
            Canvas.SetTop(LT, KeyArea.Item1.Y.DpiConvert() - LabelSize / 2);
            Canvas.SetLeft(RB, KeyArea.Item2.X.DpiConvert() - LabelSize / 2);
            Canvas.SetTop(RB, KeyArea.Item2.Y.DpiConvert() - LabelSize / 2);
        }
        public static List<(int x, int y)> SiteQuery(this List<string> source)//计算音符在点集中的坐标
        {
            var result = new List<(int x, int y)>();
            foreach (var value in source)
            {
                var x = 0;
                var y = 0;
                if (type0.Contains(value))
                {
                    x = 0;
                    for (int i = 0; i < type0.Count; i++)
                    {
                        if (type0[i] == value)
                        {
                            y = i;
                            break;
                        }
                    }
                }
                else if (type1.Contains(value))
                {
                    x = 1;
                    for (int i = 0; i < type1.Count; i++)
                    {
                        if (type1[i] == value)
                        {
                            y = i;
                            break;
                        }
                    }
                }
                else if (type2.Contains(value))
                {
                    x = 2;
                    for (int i = 0; i < type2.Count; i++)
                    {
                        if (type2[i] == value)
                        {
                            y = i;
                            break;
                        }
                    }
                }
                else
                {
                    x = 0;
                    y = 0;
                }
                result.Add((x, y));
            }


            return result;
        }
    }
}
