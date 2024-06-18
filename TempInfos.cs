using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace AutoPiano
{
    /// <summary>
    /// 临时变量，存储编辑器的状态信息
    /// </summary>
    [Serializable]
    public class TempInfos
    {
        public TempInfos() { }

        public static TempInfos? Instance;
        public static string FileName = "tempinfos";//文件名
        public static string TempInfoPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Temp");//所在文件夹


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


        #region 状态信息
        public Song? TempSong;//文本解析器内的数据
        public MetaData? MetaData;//简谱解析器内的数据
        #endregion


        #region 存取器
        public static void SaveTempInfo()
        {
            Update();

            try
            {
                Directory.CreateDirectory(TempInfoPath);

                string filePath = Path.Combine(TempInfoPath, FileName + ".bin");
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, Instance);
                }
            }
            catch { }
        }
        public static void LoadTempInfo()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(TempInfoPath, FileName + ".bin"));
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Instance = (TempInfos)binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch { }
        }
        public static void Update()
        {
            if (Instance == null) { Instance = new TempInfos(); }

            if (HotKeySet.Instance != null)
            {
                if (HotKeySet.Instance.k1 != null) Instance.A1 = HotKeySet.Instance.k1.CurrentKeyA;
                if (HotKeySet.Instance.k1 != null) Instance.A2 = HotKeySet.Instance.k1.CurrentKeyB;

                if (HotKeySet.Instance.k2 != null) Instance.B1 = HotKeySet.Instance.k2.CurrentKeyA;
                if (HotKeySet.Instance.k2 != null) Instance.B2 = HotKeySet.Instance.k2.CurrentKeyB;

                if (HotKeySet.Instance.k3 != null) Instance.C1 = HotKeySet.Instance.k3.CurrentKeyA;
                if (HotKeySet.Instance.k3 != null) Instance.C2 = HotKeySet.Instance.k3.CurrentKeyB;

                if (HotKeySet.Instance.k4 != null) Instance.D1 = HotKeySet.Instance.k4.CurrentKeyA;
                if (HotKeySet.Instance.k4 != null) Instance.D2 = HotKeySet.Instance.k4.CurrentKeyB;

                if (HotKeySet.Instance.k5 != null) Instance.E1 = HotKeySet.Instance.k5.CurrentKeyA;
                if (HotKeySet.Instance.k5 != null) Instance.E2 = HotKeySet.Instance.k5.CurrentKeyB;

                Instance.IsSameModel = HotKeySet.Instance.IsSameDataMode;
            }

            Instance.IsChangePageByClick = HotKeySet.IsClickChange;
            Instance.IsAutoAttentive = HotKeySet.IsAutoAttentive;
            Instance.IsPublicInput = TxtAnalizeVisual.IsNormalInput;
            Instance.IsPublicOutPut = TxtAnalizeVisual.IsNormalOutput;

            Instance.TempSong = TxtAnalizeVisual.CurrentSong;
        }
        public static void UseTempInfo()
        {
            if (Instance == null) { Instance = new TempInfos(); }

            TxtAnalizeVisual.IsNormalInput = Instance.IsPublicInput;
            TxtAnalizeVisual.IsNormalOutput = Instance.IsPublicOutPut;

            HotKeySet.IsClickChange = Instance.IsChangePageByClick;
            HotKeySet.IsAutoAttentive = Instance.IsAutoAttentive;

            TxtAnalizeVisual.CurrentSong = Instance.TempSong == null ? new Song() : Instance.TempSong;

            if (HotKeySet.Instance != null)
            {
                if (HotKeySet.Instance.k1 != null) HotKeySet.Instance.k1.CurrentKeyA = Instance.A1;
                if (HotKeySet.Instance.k1 != null) HotKeySet.Instance.k1.CurrentKeyB = Instance.A2;

                if (HotKeySet.Instance.k2 != null) HotKeySet.Instance.k2.CurrentKeyA = Instance.B1;
                if (HotKeySet.Instance.k2 != null) HotKeySet.Instance.k2.CurrentKeyB = Instance.B2;

                if (HotKeySet.Instance.k3 != null) HotKeySet.Instance.k3.CurrentKeyA = Instance.C1;
                if (HotKeySet.Instance.k3 != null) HotKeySet.Instance.k3.CurrentKeyB = Instance.C2;

                if (HotKeySet.Instance.k4 != null) HotKeySet.Instance.k4.CurrentKeyA = Instance.D1;
                if (HotKeySet.Instance.k4 != null) HotKeySet.Instance.k4.CurrentKeyB = Instance.D2;

                if (HotKeySet.Instance.k5 != null) HotKeySet.Instance.k5.CurrentKeyA = Instance.E1;
                if (HotKeySet.Instance.k5 != null) HotKeySet.Instance.k5.CurrentKeyB = Instance.E2;

                HotKeySet.Instance.IsSameDataMode = Instance.IsSameModel;

                HotKeySet.Instance.UpdateAfterUseTemp();
            }
        }
        #endregion
    }
}
