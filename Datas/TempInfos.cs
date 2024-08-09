using System.IO;
using System.Windows.Input;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace AutoPiano
{
    /// <summary>
    /// 编辑器的临时状态【独立读写】
    /// 简谱解析器临时存储的工作簿【】
    /// </summary>
    [Serializable]
    public class TempInfos
    {
        public TempInfos() { }

        public static TempInfos? Instance;

        public static readonly string SetFileName = "SetInfo";//设置信息
        public static readonly string TempInfoPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Status");//状态文件夹

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
        //文本解析器内的数据
        public Song? TempSong;

        //简谱解析器内的数据
        public MetaData? MetaData;
        public List<MetaData>? TempScores;
        public List<string>? TempSocresName;
        public List<string>? TempTags;
        public List<int>? Starts;
        public List<int>? Ends;
        #endregion


        #region 存取器
        public static void SaveTempInfo()
        {
            Update();

            try
            {
                Directory.CreateDirectory(TempInfoPath);

                string filePath = Path.Combine(TempInfoPath, SetFileName + ".bin");
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, Instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void LoadTempInfo()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(TempInfoPath, SetFileName + ".bin"));
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Instance = (TempInfos)binaryFormatter.Deserialize(memoryStream);
                    if (Instance != null)
                    {
                        if (Instance.TempSong != null)
                        {
                            Instance.TempSong.IsStop = false;
                            Instance.TempSong.IsOnPlaying = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void Update()
        {
            if (Instance == null) { Instance = new TempInfos(); }

            Instance.TempScores = new List<MetaData>();
            Instance.TempSocresName = new List<string>();
            Instance.TempTags = new List<string>();
            Instance.Starts = new List<int>();
            Instance.Ends = new List<int>();

            if (HotKeySet.Instance != null)
            {
                Instance.IsSameModel = HotKeySet.Instance.IsSameDataMode;

                if (NMNAnalizeVisual.Instance != null)
                {
                    MetaData temp = new MetaData();
                    temp.CopyDataFrom(NMNAnalizeVisual.Instance.MusicScore);
                    Instance.MetaData = temp;
                    Instance.Starts?.Clear();
                    Instance.Ends?.Clear();
                    foreach (var item in NMNAnalizeVisual.Instance.PrivateObjects.Children)
                    {
                        if (item is NMNAnalizeVisual.TempData DATA)
                        {
                            if (DATA.Data != null)
                            {
                                MetaData temp2 = new MetaData();
                                temp2.CopyDataFrom(DATA.Data);
                                Instance.TempScores?.Add(temp2);
                            }
                            Instance.TempTags?.Add(DATA.ButtonText);
                            Instance.TempSocresName?.Add(DATA.Name);
                            Instance.Starts?.Add(DATA.Start);
                            Instance.Ends?.Add(DATA.End);
                        }
                    }
                }
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
                HotKeySet.Instance.IsSameDataMode = Instance.IsSameModel;

                if (NMNAnalizeVisual.Instance != null && Instance.MetaData != null)
                {
                    NMNAnalizeVisual.Instance.MusicScore = Instance.MetaData.GetMusicScore();
                    NMNAnalizeVisual.Instance.SongBox.Content = NMNAnalizeVisual.Instance.MusicScore;
                    NMNAnalizeVisual.Instance.MusicScore.UpdateCoresAfterUILoaded();
                }

                if (NMNAnalizeVisual.Instance != null && Instance.TempTags != null && Instance.TempScores != null && Instance.TempSocresName != null && Instance.Starts != null && Instance.Ends != null)
                {
                    for (int i = 0; i < Instance.TempSocresName.Count; i++)
                    {
                        NMNAnalizeVisual.TempData temp = new NMNAnalizeVisual.TempData();
                        temp.Data = Instance.TempScores[i].GetMusicScore();
                        temp.ButtonText = Instance.TempTags[i];
                        temp.Name = Instance.TempSocresName[i];
                        temp.Start = Instance.Starts[i];
                        temp.End = Instance.Ends[i];
                        NMNAnalizeVisual.Instance.PrivateObjects.Children.Add(temp);
                    }
                }
            }
        }
        #endregion
    }
}
