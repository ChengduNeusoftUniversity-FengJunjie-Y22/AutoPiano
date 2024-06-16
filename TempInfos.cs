using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoPiano
{
    /// <summary>
    /// 临时变量，存储编辑器的状态信息
    /// </summary>
    [Serializable]
    internal class TempInfos
    {
        public TempInfos() { }

        #region  热键Keys
        public Key A1;
        public Key A2;
        //开始

        public Key B1;
        public Key B2;
        //暂停

        public Key C1;
        public Key C2;
        //停止

        public Key D1;
        public Key D2;
        //视窗

        public Key E1;
        public Key E2;
        //隐藏视窗（不关闭）
        #endregion


        #region  流程控制bool值
        public bool IsChangePageByClick = false;//切页模式

        public bool IsAutoAttentive = true;//自动专注

        public bool IsPublicInput = true;//公共读取协议

        public bool IsPublicOutPut = true;//公共输出协议

        public bool IsSameModel = true;//协议同步
        #endregion


        #region 临时变量
        public Song? TempSong;//文本解析器内的数据

        public NumberedMusicalNotation.MusicScore? TempMusicScore;//简谱解析器内的数据
        #endregion


        #region 状态信息存取器
        public static string TempInfoPath = System.IO.Path.Combine();
        #endregion
    }
}
