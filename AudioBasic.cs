﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WindowsInput;
using WindowsInput.Native;

/// <summary>
/// 【枚举】乐器类型
/// </summary>
public enum InstrumentTypes : int
{
    /// <summary>
    /// 风物之诗琴
    /// </summary>
    FWPiano = 21,

    /// <summary>
    /// 晚风圆号
    /// </summary>
    WFHorn = 14,

    /// <summary>
    /// 镜花之琴
    /// </summary>
    JHPiano = 21,

    /// <summary>
    /// 荒泷盛世豪鼓
    /// </summary>
    HLDrum = 4,

    /// <summary>
    /// 老旧的诗琴
    /// </summary>
    XMPiano = 21,

    /// <summary>
    /// ⚠类型不明
    /// </summary>
    None = 0
}

namespace AutoPiano
{
    /// <summary>
    /// 【抽象类】最为基础的音乐依赖
    /// </summary>
    internal abstract class AudioBasic : MusicTheory
    {
        #region 音源控制模块
        public static readonly string AudioForFWPiano = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_FW");
        public static readonly string AudioForWFHorn = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_WF");
        public static readonly string AudioForJHPiano = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_JH");
        public static readonly string AudioForHLDrum = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_HL");
        public static readonly string AudioForXMPiano = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_XM");
        public static readonly VirtualKeyCode[] AdudioKey = new VirtualKeyCode[]
        {
            VirtualKeyCode.VK_Q,
            VirtualKeyCode.VK_W,
            VirtualKeyCode.VK_E,
            VirtualKeyCode.VK_R,
            VirtualKeyCode.VK_T,
            VirtualKeyCode.VK_Y,
            VirtualKeyCode.VK_U,

            VirtualKeyCode.VK_A,
            VirtualKeyCode.VK_S,
            VirtualKeyCode.VK_D,
            VirtualKeyCode.VK_F,
            VirtualKeyCode.VK_G,
            VirtualKeyCode.VK_H,
            VirtualKeyCode.VK_J,

            VirtualKeyCode.VK_Z,
            VirtualKeyCode.VK_X,
            VirtualKeyCode.VK_C,
            VirtualKeyCode.VK_V,
            VirtualKeyCode.VK_B,
            VirtualKeyCode.VK_N,
            VirtualKeyCode.VK_M,
        };
        /// <summary>
        /// 约定所有乐器的文件名必须是【Key+.mp3】
        /// </summary>
        public static readonly string[] AudioName = new string[]
        {
        "Q.mp3",
        "W.mp3",
        "E.mp3",
        "R.mp3",
        "T.mp3",
        "Y.mp3",
        "U.mp3",

        "A.mp3",
        "S.mp3",
        "D.mp3",
        "F.mp3",
        "G.mp3",
        "H.mp3",
        "J.mp3",

        "Z.mp3",
        "X.mp3",
        "C.mp3",
        "V.mp3",
        "B.mp3",
        "N.mp3",
        "M.mp3",
        };
        /// <summary>
        /// 检查预览音频支持文件夹是否完备
        /// </summary>
        public static void CheckAudioFolder()
        {
            if (!System.IO.Directory.Exists(AudioForFWPiano))
            {
                System.IO.Directory.CreateDirectory(AudioForFWPiano);
            }
            if (!System.IO.Directory.Exists(AudioForWFHorn))
            {
                System.IO.Directory.CreateDirectory(AudioForWFHorn);
            }
            if (!System.IO.Directory.Exists(AudioForJHPiano))
            {
                System.IO.Directory.CreateDirectory(AudioForJHPiano);
            }
            if (!System.IO.Directory.Exists(AudioForHLDrum))
            {
                System.IO.Directory.CreateDirectory(AudioForHLDrum);
            }
            if (!System.IO.Directory.Exists(AudioForXMPiano))
            {
                System.IO.Directory.CreateDirectory(AudioForXMPiano);
            }
        }
        /// <summary>
        /// 键盘操作模拟器
        /// </summary>
        public static InputSimulator Simulator = new InputSimulator();
        /// <summary>
        /// 切换乐器时，请更新字典，这是能够正确进行预览操作的前提
        /// </summary>
        public static Dictionary<VirtualKeyCode, MediaPlayer> KeyToMediaPlayer = new Dictionary<VirtualKeyCode, MediaPlayer>();
        /// <summary>
        /// 依据按键码模拟用户输入
        /// </summary>
        public static void PlayWithKeyCode(VirtualKeyCode target)
        {
            MediaPlayer? player = null;
            KeyToMediaPlayer.TryGetValue(target, out player);
            if (player != null) { player.Stop(); player.Position = TimeSpan.Zero; player.Play(); }
        }
        public static void PlayWithInt(int key)
        {
            VirtualKeyCode keyCode;
            IntToKeyCode.TryGetValue(key, out keyCode);
            MediaPlayer? player = null;
            KeyToMediaPlayer.TryGetValue(keyCode, out player);
            if (player != null) { player.Stop(); player.Position = TimeSpan.Zero; player.Play(); }
        }
        public static void Operation()
        {

        }
        /// <summary>
        /// 依据乐器类型更新音源字典
        /// </summary>
        /// <param name="type"></param>
        public static void UpdateAudioByType(InstrumentTypes type)
        {
            Dictionary<VirtualKeyCode, MediaPlayer> result = new Dictionary<VirtualKeyCode, MediaPlayer>();
            switch (type)
            {
                case InstrumentTypes.FWPiano:
                    for (int i = 0; i < (int)type; i++)
                    {
                        MediaPlayer mediaPlayer = new MediaPlayer();
                        mediaPlayer.Open(new Uri(System.IO.Path.Combine(AudioForFWPiano + AudioName[i])));
                        result.Add(AdudioKey[i], mediaPlayer);
                    }
                    KeyToMediaPlayer = result;
                    break;
                case InstrumentTypes.WFHorn:
                    for (int i = 0; i < (int)type; i++)
                    {
                        MediaPlayer mediaPlayer = new MediaPlayer();
                        mediaPlayer.Open(new Uri(System.IO.Path.Combine(AudioForWFHorn + AudioName[i])));
                        result.Add(AdudioKey[i], mediaPlayer);
                    }
                    KeyToMediaPlayer = result;
                    break;
            }
        }
        #endregion

        #region 各类解析过程中，需要的转化
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
        #endregion
    }
}