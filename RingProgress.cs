using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace AutoPiano
{
    public partial class RingProgress : RangeBase
    {

        static RingProgress()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RingProgress), new FrameworkPropertyMetadata(typeof(RingProgress)));
        }

        #region StrokeThickness 圆环描边宽度
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RingProgress), new PropertyMetadata(10d));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        #endregion

        #region Stroke 圆环描边颜色
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(RingProgress), new PropertyMetadata(Brushes.Red));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        #endregion
    }

    internal class RingProgressArcConverter : IMultiValueConverter
    {
        // 注意,因为这里使用Path绘制圆环, 所以要把描边宽度大小考虑进去. 所有点的x、y偏移 半个描边宽度
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double value;
            if (values[0] is double width
                && values[1] is double height
                && width > 0 && height > 0
                && values[2] is double storkeWidth)
            {
                width -= storkeWidth;
                height -= storkeWidth;

                value = values.Length == 4 ? System.Convert.ToDouble(values[3]) : 1d;

                if (value == 0) return "";

                var startAngle = -90d;
                var endAngle = Math.Min(value * 360 - 90, 269);
                var radius = Math.Min(width, height) * 0.5;
                var start = startAngle.AngleToPoint(radius, storkeWidth * 0.5);
                var end = endAngle.AngleToPoint(radius, storkeWidth * 0.5);

                var dataStr = $"M {start.X},{start.Y} A {radius},{radius} 0 {(endAngle - startAngle >= 180 ? 1 : 0)} 1 {end.X},{end.Y}";

                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                return (Geometry)converter.ConvertFrom(dataStr);
            }
            else
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class RingProgressValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double v)
            {
                return $"{v * 100}%";
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal static class RingProgressExtension
    {
        /// <summary>
        /// 角度转为弧度
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double AngleToArc(this double a)
        {
            return Math.PI * a / 180;
        }
        /// <summary>
        /// 角度及半径计算坐标点位置
        /// </summary>
        /// <param name="a"></param>
        /// <param name="radius"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Point AngleToPoint(this double a, double radius, double offset = 0)
        {
            return new Point(Math.Cos(a.AngleToArc()) * radius + radius + offset, Math.Sin(a.AngleToArc()) * radius + radius + offset);
        }
    }
}
