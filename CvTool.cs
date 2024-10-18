using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Drawing;
using System.Linq;
using System.Windows;
using MinimalisticWPF;
using System.IO;

namespace AutoPiano
{
    public static class CvTool
    {
        static string templates = "templates";

        public static (Point2f, Point2f) GetArea()
        {
            // 获取屏幕图像
            Mat Screen = ScreenRead();

            // 获取圆心点列表
            var Points = CircleFind(Screen);

            // 使用 LINQ 找到最左上和最右下的点
            var topLeft = Points.OrderBy(p => p.X).ThenBy(p => p.Y).First();
            var bottomRight = Points.OrderByDescending(p => p.X).ThenByDescending(p => p.Y).First();

            return (topLeft, bottomRight);
        }

        /// <summary>
        /// 霍夫圆检测
        /// </summary>
        public static List<Point2f> CircleFind(Mat target, int minRadius = 50, int maxRadius = 100)
        {
            var result = new List<Point2f>();

            Mat src = new Mat();
            target.CopyTo(src);
            Mat gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            //高斯模糊
            Mat blurred = new Mat();
            Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(9, 9), 2, 2);

            //霍夫圆检测
            CircleSegment[] circles = Cv2.HoughCircles(blurred, HoughModes.Gradient, dp: 1, minDist: 20, param1: 150, param2: 35, minRadius: minRadius, maxRadius: maxRadius);

            //结果
            foreach (var circle in circles)
            {
                result.Add(circle.Center);
            }

            return result;
        }

        /// <summary>
        /// 获取屏幕截图
        /// </summary>
        public static Mat ScreenRead()
        {
            var screenSize = GetScreenSize();
            Bitmap screenshot = new Bitmap(screenSize.Item1, screenSize.Item2);

            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenSize.Item1, screenSize.Item2));
            }

            Mat mat = BitmapSourceConverter.ToMat(screenshot.ToBitmapSource());
            return mat;
        }

        /// <summary>
        /// 获取屏幕物理尺寸
        /// </summary>
        /// <returns>Item1/Item2 宽度/高度</returns>
        public static (int, int) GetScreenSize()
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            double dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            int screen_width = (int)(SystemParameters.PrimaryScreenWidth * dpiX / 96.0);
            int screen_height = (int)(SystemParameters.PrimaryScreenHeight * dpiY / 96.0);
            return (screen_width, screen_height);
        }

        public static double DpiConvert(this double local, double rate = 1)
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpiX = source.CompositionTarget.TransformToDevice.M11;
            double dpiY = source.CompositionTarget.TransformToDevice.M22;
            double newValue = local * (1 - 1 / (2 * dpiX));
            return newValue * rate;
        }

        public static double DpiConvert(this int local, double rate = 1)
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpiX = source.CompositionTarget.TransformToDevice.M11;
            double dpiY = source.CompositionTarget.TransformToDevice.M22;
            double newValue = local * (1 - 1 / (2 * dpiX));
            return newValue * rate;
        }

        public static double DpiConvert(this float local, double rate = 1)
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpiX = source.CompositionTarget.TransformToDevice.M11;
            double dpiY = source.CompositionTarget.TransformToDevice.M22;
            double newValue = local * (1 - 1 / (2 * dpiX));
            return newValue * rate;
        }
    }
}
