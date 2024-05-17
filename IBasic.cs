using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace AutoPiano
{
    /// <summary>
    /// 【接口】最为基础的模块
    /// </summary>
    internal interface IBasic
    {
        /// <summary>
        /// 【接口】键盘操作模拟器
        /// </summary>
        public static InputSimulator Simulator = new InputSimulator();

        #region 字典
        /// <summary>
        /// [键盘char]  映射到  [虚拟按键VirtualKeyCode]
        /// </summary>
        public static Dictionary<char, VirtualKeyCode> CharToKeyCode = new Dictionary<char, VirtualKeyCode>//操作 → 虚拟按键
        {
        {'Q', VirtualKeyCode.VK_Q},
        {'W', VirtualKeyCode.VK_W},
        {'E', VirtualKeyCode.VK_E},
        {'R', VirtualKeyCode.VK_R},
        {'T', VirtualKeyCode.VK_T},
        {'Y', VirtualKeyCode.VK_Y},
        {'U', VirtualKeyCode.VK_U},

        {'A', VirtualKeyCode.VK_A},
        {'S', VirtualKeyCode.VK_S},
        {'D', VirtualKeyCode.VK_D},
        {'F', VirtualKeyCode.VK_F},
        {'G', VirtualKeyCode.VK_G},
        {'H', VirtualKeyCode.VK_H},
        {'J', VirtualKeyCode.VK_J},

        {'Z', VirtualKeyCode.VK_Z},
        {'X', VirtualKeyCode.VK_X},
        {'C', VirtualKeyCode.VK_C},
        {'V', VirtualKeyCode.VK_V},
        {'B', VirtualKeyCode.VK_B},
        {'N', VirtualKeyCode.VK_N},
        {'M', VirtualKeyCode.VK_M},
        };

        /// <summary>
        /// [虚拟按键VirtualKeyCode]  映射到  [键盘char]
        /// </summary>
        public static Dictionary<VirtualKeyCode, char> KeyCodeToChar = CharToKeyCode.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        /// [音阶int]  映射到  [虚拟按键VirtualKeyCode]
        /// </summary>
        public static Dictionary<int, VirtualKeyCode> IntToKeyCode = new Dictionary<int, VirtualKeyCode>()//音阶→虚拟按键
        {
        {8, VirtualKeyCode.VK_Q},
        {9, VirtualKeyCode.VK_W},
        {10, VirtualKeyCode.VK_E},
        {11, VirtualKeyCode.VK_R},
        {12, VirtualKeyCode.VK_T},
        {13, VirtualKeyCode.VK_Y},
        {14, VirtualKeyCode.VK_U},

        {1, VirtualKeyCode.VK_A},
        {2, VirtualKeyCode.VK_S},
        {3, VirtualKeyCode.VK_D},
        {4, VirtualKeyCode.VK_F},
        {5, VirtualKeyCode.VK_G},
        {6, VirtualKeyCode.VK_H},
        {7, VirtualKeyCode.VK_J},

        {-1, VirtualKeyCode.VK_Z},
        {-2, VirtualKeyCode.VK_X},
        {-3, VirtualKeyCode.VK_C},
        {-4, VirtualKeyCode.VK_V},
        {-5, VirtualKeyCode.VK_B},
        {-6, VirtualKeyCode.VK_N},
        {-7, VirtualKeyCode.VK_M},

        {0, VirtualKeyCode.VK_P},
        };
        /// <summary>
        /// [音阶int]  映射到  [无高低音的字符表示string]
        /// </summary>
        public static Dictionary<int, string> IntToCoreString = new Dictionary<int, string>()//音阶→字符
        {
        {-1, "1"},
        {-2, "2"},
        {-3, "3"},
        {-4, "4"},
        {-5, "5"},
        {-6, "6"},
        {-7, "7"},
        {1, "1"},
        {2, "2"},
        {3, "3"},
        {4, "4"},
        {5, "5"},
        {6, "6"},
        {7, "7"},
        {8, "1"},
        {9, "2"},
        {10, "3"},
        {11, "4"},
        {12, "5"},
        {13, "6"},
        {14, "7"},
        };

        /// <summary>
        /// [高音int]  映射到  [中音int]
        /// </summary>
        public static Dictionary<int, int> TopToCenter = new Dictionary<int, int>()
        {
        {8, 1},
        {9, 2},
        {10, 3},
        {11, 4},
        {12, 5},
        {13, 6},
        {14, 7},
        };
        /// <summary>
        /// [高音int]  映射到  [低音int]
        /// </summary>
        public static Dictionary<int, int> TopToLow = new Dictionary<int, int>()
        {
        {8, -1},
        {9, -2},
        {10, -3},
        {11, -4},
        {12, -5},
        {13, -6},
        {14, -7},
        };

        /// <summary>
        /// [低音int]  映射到  [中音int]
        /// </summary>
        public static Dictionary<int, int> LowToCenter = new Dictionary<int, int>()
        {
        {-1, 1},
        {-2, 2},
        {-3, 3},
        {-4, 4},
        {-5, 5},
        {-6, 6},
        {-7, 7},
        };
        /// <summary>
        /// [低音int]  映射到  [高音int]
        /// </summary>
        public static Dictionary<int, int> LowToTop = TopToLow.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        /// [中音int]  映射到  [高音int]
        /// </summary>
        public static Dictionary<int, int> CenterToTop = TopToCenter.ToDictionary(x => x.Value, x => x.Key);
        /// <summary>
        /// [中音int]  映射到  [低音int]
        /// </summary>
        public static Dictionary<int, int> CenterToLow = LowToCenter.ToDictionary(x => x.Value, x => x.Key);
        #endregion

        public static VirtualKeyCode GetKeyCode(char target)
        {
            VirtualKeyCode result = new VirtualKeyCode();
            CharToKeyCode.TryGetValue(target, out result);
            return result;
        }

        public static char GetKeyChar(VirtualKeyCode keyCode)
        {
            char result = new char();
            KeyCodeToChar.TryGetValue(keyCode, out result);
            return result;
        }
    }
}
