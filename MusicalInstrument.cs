
using System.Security.Permissions;
using System.Windows.Input;
using System.Windows.Media;
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
    /// 【接口】乐器
    /// </summary>
    internal abstract class MusicalInstrument : Basic
    {

        /// <summary>
        /// 【接口】乐器类型
        /// </summary>
        public static InstrumentTypes InstrumentType { get; }
    }
}
