using Microsoft.Expression.Shapes;
using MinimalisticWPF;
using OpenCvSharp;
using System;
using System.Collections.Generic;
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
        private static double labelsize = 100;
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
        public static void SetPopupScreen()
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
        public static void UpdateArea()
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
                    Positions[i, j] = new Point2f(lt.X + deltaX * j, rb.Y + deltaY * i);
                }
            }
            Canvas.SetLeft(LT, KeyArea.Item1.X.DpiConvert() - LabelSize / 2);
            Canvas.SetTop(LT, KeyArea.Item1.Y.DpiConvert() - LabelSize / 2);
            Canvas.SetLeft(RB, KeyArea.Item2.X.DpiConvert() - LabelSize / 2);
            Canvas.SetTop(RB, KeyArea.Item2.Y.DpiConvert() - LabelSize / 2);
        }
    }
}
