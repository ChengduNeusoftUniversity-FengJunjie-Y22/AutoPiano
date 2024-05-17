
using System.Security.Permissions;
using System.Windows.Input;
using System.Windows.Media;

/// <summary>
/// 【枚举】乐器类型
/// </summary>
public enum InstrumentTypes
{
    /// <summary>
    /// 风物之诗琴
    /// </summary>
    FWPiano,

    /// <summary>
    /// 晚风圆号
    /// </summary>
    WFHorn,

    /// <summary>
    /// 镜花之琴
    /// </summary>
    JHPiano,

    /// <summary>
    /// 荒泷盛世豪鼓
    /// </summary>
    HLDrum,

    /// <summary>
    /// 老旧的诗琴
    /// </summary>
    XMPiano,

    /// <summary>
    /// ⚠类型不明
    /// </summary>
    None
}

namespace AutoPiano
{
    /// <summary>
    /// 【接口】乐器
    /// </summary>
    internal interface IMusicalInstrument
    {
        /// 预览功能的音频支持文件地址
        public static string AudioForFWPiano = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_FW");
        public static string AudioForWFHorn = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_WF");
        public static string AudioForJHPiano = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_JH");
        public static string AudioForHLDrum = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_HL");
        public static string AudioForXMPiano = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Audio_XM");

        /// <summary>
        /// 切换乐器时，请更新字典
        /// </summary>
        public static Dictionary<Key, MediaPlayer> KeyToMediaPlayer = new Dictionary<Key, MediaPlayer>();

        /// <summary>
        /// 【接口】乐器类型
        /// </summary>
        public static InstrumentTypes InstrumentType { get; }

        public static void CheckAudioFolder()
        {

        }
    }
}
