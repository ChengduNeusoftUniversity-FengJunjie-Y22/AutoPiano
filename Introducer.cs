using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

/// <summary>
/// 枚举快捷键提示的类型
/// </summary>
public enum IntroduceType
{
    KeyParse,
    ViewParse_New,
    ViewParse_Add,
    ViewParse_Remove,
    ViewParse_Sum,
    AutoPlay,
    Left,
    Guider,
    Right
}

namespace AutoPiano
{
    internal class Introducer : Grid
    {
        public Introducer()
        {
            Width = double.NaN;
            Height = double.NaN;
        }

        public StackPanel sp = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            Width = double.NaN,
            Height = double.NaN,
            Background = Brushes.White,
        };

        public TextBlock tb_L_1 = new TextBlock()
        {
            Text = "Tips:\n【鼠标左/右】      > 添加音轨/播放小节",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_L_2 = new TextBlock()
        {
            Text = "【CTRL+鼠标左/右】 > 复制/删除小节",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };


        public TextBlock tb_G_1 = new TextBlock()
        {
            Text = "Tips:\n【鼠标左】  >  切换至指定工作簿",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_G_2 = new TextBlock()
        {
            Text = "【鼠标右】  >  将指定工作簿复制到当前工作簿的末尾",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };


        public TextBlock tb_R_1 = new TextBlock()
        {
            Text = "【Tips:\n鼠标左/右】    > 删除/播放音轨",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_R_2 = new TextBlock()
        {
            Text = "【滚轮⇧/⇩】      > 增删音符",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_R_3 = new TextBlock()
        {
            Text = "【CTRL+滚轮⇧/⇩】 > 增删占位音符",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        public TextBlock tb_view_0 = new TextBlock()
        {
            Text = "【CTRL+滚轮⇧/⇩】 > 增删占位音符",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        public TextBlock tb_alt_0 = new TextBlock()
        {
            Text = "Tips:\n【Alt+Space】 > 播放开关",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_alt_1 = new TextBlock()
        {
            Text = "【Alt+⇦/⇨】或【Ctrl+鼠标左右】 >  缩短/延长音符的时值",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_alt_2 = new TextBlock()
        {
            Text = "【Alt+⇧/⇩】或【滚轮⇧/⇩】 > 向上/向下切换音符",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_alt_3 = new TextBlock()
        {
            Text = "【鼠标左/右】 >  选中/预览单个音符",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };


        public TextBlock tb_auto_0 = new TextBlock()
        {
            Text = "Tips:\n【Ctrl+F1】 > 自动演奏开关（Alt+Space可能与NVIDIA冲突）",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_auto_1 = new TextBlock()
        {
            Text = "【Ctrl+F2】 > 弹窗控制",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_auto_2 = new TextBlock()
        {
            Text = "【Ctrl+⇧/⇩】 > 向上/向下切换歌曲",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_auto_3 = new TextBlock()
        {
            Text = "【鼠标左/右】 >  选中歌曲/将歌曲回放到<键盘>调试中二次调试",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };


        public TextBlock tb_New = new TextBlock()
        {
            Text = "Tips;\n【✧】 >  新建一个包含4小节的简谱",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_A = new TextBlock()
        {
            Text = "Tips;\n【+】 >  在末尾添加一个默认小节",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_R = new TextBlock()
        {
            Text = "Tips;\n【-】 >  从末尾删除一个小节",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        public TextBlock tb_S = new TextBlock()
        {
            Text = "Tips;\n【Σ】 >  将当前数据连接至指定歌曲的末尾",
            Background = Brushes.Transparent,
            Foreground = Brushes.Black,
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Left,
        };


        public Introducer GetIntroduce(IntroduceType type)
        {
            Children.Clear();
            sp.Children.Clear();
            switch (type)
            {
                case IntroduceType.KeyParse:
                    sp.Children.Add(tb_alt_0);
                    sp.Children.Add(tb_alt_1);
                    sp.Children.Add(tb_alt_2);
                    sp.Children.Add(tb_alt_3);
                    break;
                case IntroduceType.AutoPlay:
                    sp.Children.Add(tb_auto_0);
                    sp.Children.Add(tb_auto_1);
                    sp.Children.Add(tb_auto_2);
                    sp.Children.Add(tb_auto_3);
                    break;
                case IntroduceType.Left:
                    sp.Children.Add(tb_L_1);
                    sp.Children.Add(tb_L_2);
                    break;
                case IntroduceType.Guider:
                    sp.Children.Add(tb_G_1);
                    sp.Children.Add(tb_G_2);
                    break;
                case IntroduceType.Right:
                    sp.Children.Add(tb_R_1);
                    sp.Children.Add(tb_R_2);
                    sp.Children.Add(tb_R_3);
                    break;
                case IntroduceType.ViewParse_New:
                    sp.Children.Add(tb_New);
                    break;
                case IntroduceType.ViewParse_Add:
                    sp.Children.Add(tb_A);
                    break;
                case IntroduceType.ViewParse_Remove:
                    sp.Children.Add(tb_R);
                    break;
                case IntroduceType.ViewParse_Sum:
                    sp.Children.Add(tb_S);
                    break;
            }
            Children.Add(sp);
            return this;
        }
    }
}
