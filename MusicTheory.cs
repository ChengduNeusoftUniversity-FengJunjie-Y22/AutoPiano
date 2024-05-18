using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoPiano.NumberedMusicalNotation;

namespace AutoPiano
{
    /// <summary>
    /// 【乐理基础】
    /// </summary>
    public abstract class MusicTheory
    {
        /// <summary>
        /// 只能同时进行一个与播放有关的操作
        /// </summary>
        public static bool IsWorking = false;

        /// <summary>
        /// 是否要停止目前进行中的预览OR自动演奏
        /// </summary>
        public static bool IsWorkStop = false;

        /// <summary>
        /// 速度
        /// </summary>
        private static int _speed = 80;
        public static int Speed
        {
            get { return _speed; }
            set
            {
                if (value > 0)
                {
                    _speed = value;
                }
                else
                {
                    _speed = 150;
                }
                ReCalculateSets();
            }
        }

        /// <summary>
        /// 【_leftnum/_rightnum】表示【以_rightnum分音符为一拍,每小节有_leftnum拍】 > 【默认4/4拍】
        /// </summary>
        private static int _leftnum = 4;
        private static int _rightnum = 4;
        public static int LeftNum
        {
            get { return _leftnum; }
            set
            {
                if (value > 0)
                {
                    _leftnum = value;
                }
                else
                {
                    _leftnum = 4;
                }
            }
        }
        public static int RightNum
        {
            get { return _rightnum; }
            set
            {
                if (value > 0)
                {
                    _rightnum = value;
                }
                else
                {
                    _rightnum = 4;
                }
                ReCalculateSets();
            }
        }

        /// <summary>
        /// 重新计算一些必要的值
        /// </summary>
        public static void ReCalculateSets()
        {
            _basicvalue = (int)((60000f / Speed) * LeftNum); // 一小节的持续时间，考虑了拍号
            TypeToValue = new Dictionary<int, int>()//血泪教训，按值传递，要及时刷新！！！
            {
            {1,WholeNote },
            {2,HalfNote },
            {4,QuarterNote },
            {8,EighthNote },
            {16,SixteenthNote },
            { 32,FastNote}
            };
        }

        /// <summary>
        /// 基于此值获取不同音符的绝对时值
        /// </summary>
        private static int _basicvalue = 3000;
        public static int BasicValue
        {
            get { return _basicvalue; }
            set
            {
                if (value > 0)
                {
                    _basicvalue = value;
                    return;
                }
                _basicvalue = 3000;
            }
        }

        /// <summary>
        /// 全音符
        /// </summary>
        public static int WholeNote { get { return _basicvalue; } }

        /// <summary>
        /// 二分音符
        /// </summary>
        public static int HalfNote { get { return _basicvalue / 2; } }

        /// <summary>
        /// 四分音符
        /// </summary>
        public static int QuarterNote { get { return _basicvalue / 4; } }

        /// <summary>
        /// 八分音符
        /// </summary>
        public static int EighthNote { get { return _basicvalue / 8; } }

        /// <summary>
        /// 十六分音符
        /// </summary>
        public static int SixteenthNote { get { return _basicvalue / 16; } }

        /// <summary>
        /// 连音符号(不计入时值类型)
        /// </summary>
        public static int FastNote { get { return _basicvalue / 32; } }

        /// <summary>
        /// 便于检查音符类型是否合法
        /// </summary>
        public static int[] NoteTypes = new int[] { 1, 2, 4, 8, 16, 32 };

        /// <summary>
        ///【时值类型】 映射到 【时值】
        /// </summary>
        public static Dictionary<int, int> TypeToValue = new Dictionary<int, int>()
        {
            {1,WholeNote },
            {2,HalfNote },
            {4,QuarterNote },
            {8,EighthNote },
            {16,SixteenthNote },
            { 32,FastNote}
        };

        /// <summary>
        /// 规避int按值传递，实现字典数据动态更新
        /// </summary>
        /// <param name="core">需要得到正确时值的音符Core</param>
        /// <returns>时值int</returns>
        public static int GetTimeValueByType(Core core)
        {
            if (TypeToValue.ContainsKey(core.Type))
            {
                return TypeToValue[core.Type];
            }
            core.Type = 1;
            return _basicvalue;
        }
    }
}
