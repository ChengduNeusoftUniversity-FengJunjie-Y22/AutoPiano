
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
        /// <summary>
        /// 【接口】乐器类型
        /// </summary>
        InstrumentTypes InstrumentType { get; }
    }
}
