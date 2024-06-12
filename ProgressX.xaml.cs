using AutoPiano;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// 用于在外部为进度条定义一些行为
/// </summary>
/// <param name="progressX">传出进度条自身引用</param>
public delegate void ProgressEvent(ProgressX progressX);

namespace AutoPiano
{
    public partial class ProgressX : UserControl
    {
        public ProgressX()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 正式传入值之前，可以做点必要的准备
        /// </summary>
        public ProgressEvent? BeforeSetValue;

        /// <summary>
        /// 值传入结束后，可以再触发点什么
        /// </summary>
        public ProgressEvent? AfterSetValue;

        /// <summary>
        /// 进度比率 0~1 => 1%~100%
        /// <para>若想传入参数,请调用SetProgressValue</para>
        /// </summary>
        public double ProcessRate
        {
            get => Bar.Value;
        }

        /// <summary>
        /// 进度条底色
        /// </summary>
        public SolidColorBrush BackColor
        {
            set => Bar.Background = value;
        }

        /// <summary>
        /// 进度条填充色、百分比文字色
        /// </summary>
        public SolidColorBrush ForeColor
        {
            set => Bar.Foreground = value;
        }

        /// <summary>
        /// 设置进度比率值
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="value">进度比率</param>
        public void SetValue(double value)
        {
            BeforeSetValue?.Invoke(this);
            Bar.Value = value;
            AfterSetValue?.Invoke(this);
        }
    }
}
