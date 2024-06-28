using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Serialization;
using System.Xml.XPath;

public enum DataTypes
{
    PrivateAutoData,
    PrivateVisualData,
    PublicAutoData,
    PublicVisualData
}

namespace AutoPiano
{
    [Serializable]
    /// <summary>
    /// 【抽象类】数据读写
    /// </summary>
    public static class FileTool
    {
        //文件夹 - 1级 - 总文件夹
        public static readonly string DataFloder = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data");

        //文件夹 - 2级 - 公私格式的解析结果数据
        public static readonly string PublicData = System.IO.Path.Combine(DataFloder, "Public");
        public static readonly string PrivateData = System.IO.Path.Combine(DataFloder, "Private");
        public static readonly string AudioSource = System.IO.Path.Combine(DataFloder, "AudioSource");

        //文件夹 - 3级 - 具体的数据
        public static readonly string PublicAutoData = System.IO.Path.Combine(PublicData, "AutoData");
        public static readonly string PublicVisualData = System.IO.Path.Combine(PublicData, "VisualData");
        public static readonly string PrivateAutoData = System.IO.Path.Combine(PrivateData, "AutoData");
        public static readonly string PrivateVisualData = System.IO.Path.Combine(PrivateData, "VisualData");
        public static readonly string AudioForFW = System.IO.Path.Combine(AudioSource, "Audio_FW");
        public static readonly string AudioForWF = System.IO.Path.Combine(AudioSource, "Audio_WF");
        public static readonly string AudioForJH = System.IO.Path.Combine(AudioSource, "Audio_JH");
        public static readonly string AudioForHL = System.IO.Path.Combine(AudioSource, "Audio_HL");
        public static readonly string AudioForXM = System.IO.Path.Combine(AudioSource, "Audio_XM");

        //用户上次选择的文件夹
        public static string? TempSelectedFloder = null;

        public static void CheckDataFloder()
        {
            if (!System.IO.Directory.Exists(DataFloder))
            {
                System.IO.Directory.CreateDirectory(DataFloder);
            }

            if (!System.IO.Directory.Exists(PublicData))
            {
                System.IO.Directory.CreateDirectory(PublicData);
            }
            if (!System.IO.Directory.Exists(PrivateData))
            {
                System.IO.Directory.CreateDirectory(PrivateData);
            }
            if (!System.IO.Directory.Exists(AudioSource))
            {
                System.IO.Directory.CreateDirectory(AudioSource);
            }

            if (!System.IO.Directory.Exists(PublicAutoData))
            {
                System.IO.Directory.CreateDirectory(PublicAutoData);
            }
            if (!System.IO.Directory.Exists(PublicVisualData))
            {
                System.IO.Directory.CreateDirectory(PublicVisualData);
            }
            if (!System.IO.Directory.Exists(PrivateAutoData))
            {
                System.IO.Directory.CreateDirectory(PrivateAutoData);
            }
            if (!System.IO.Directory.Exists(PrivateVisualData))
            {
                System.IO.Directory.CreateDirectory(PrivateVisualData);
            }
            if (!System.IO.Directory.Exists(AudioForFW))
            {
                System.IO.Directory.CreateDirectory(AudioForFW);
            }
            if (!System.IO.Directory.Exists(AudioForWF))
            {
                System.IO.Directory.CreateDirectory(AudioForWF);
            }
            if (!System.IO.Directory.Exists(AudioForJH))
            {
                System.IO.Directory.CreateDirectory(AudioForJH);
            }
            if (!System.IO.Directory.Exists(AudioForHL))
            {
                System.IO.Directory.CreateDirectory(AudioForHL);
            }
            if (!System.IO.Directory.Exists(AudioForXM))
            {
                System.IO.Directory.CreateDirectory(AudioForXM);
            }
        }

        /// <summary>
        /// 依据对象实例，数据类型，名称，以指定协议存储对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="binobject">对象实例</param>
        /// <param name="type">数据类型</param>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static bool SerializeObject<T>(T binobject, DataTypes type, string name) where T : class, new()
        {
            string floderPath = ParseFloderByDataType(type);
            string filePath = string.Empty;

            filePath = System.IO.Path.Combine(floderPath, name + ".bin");

            if (type == DataTypes.PublicAutoData)
            {
                try
                {
                    if (binobject is Song song)
                    {
                        File.WriteAllText(filePath, StringProcessing.SongToNormalData(song));
                    }
                }
                catch { return false; }
            }
            else if (type == DataTypes.PublicVisualData)
            {
                try
                {
                    if (binobject is MetaData meta)
                    {
                        File.WriteAllText(filePath, StringProcessing.MetaDataToNormalData(meta));
                    }
                }
                catch { return false; }
            }
            else
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();

                        binaryFormatter.Serialize(memoryStream, binobject);

                        File.WriteAllBytes(filePath, memoryStream.ToArray());
                    }
                    return true;
                }
                catch { return false; }
            }

            return false;
        }

        public static bool SerializeObject(MetaData binobject, string filePath, bool IsTextOutput)
        {
            if (IsTextOutput)
            {
                try
                {
                    File.WriteAllText(filePath, StringProcessing.MetaDataToNormalData(binobject));
                    return true;
                }
                catch { return false; }
            }
            else
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fs, binobject);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 依据数据类型，打开指定文件夹，读取指定文件
        /// </summary>
        public static (bool, T) DeserializeObject<T>(DataTypes type) where T : class, new()
        {
            string floderPath = ParseFloderByDataType(type);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = floderPath;
            openFileDialog.Title = "在此文件夹下选择数据文件";
            openFileDialog.Filter = ParseFilterByDataType(type);

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                if (filePath == null) { return (false, new T()); }

                if (type == DataTypes.PublicAutoData)
                {
                    if (StringProcessing.NormalDataToSong(File.ReadAllText(filePath)) is T temp)
                    {
                        return (true, temp);
                    }
                }
                else if (type == DataTypes.PublicVisualData)
                {
                    if (StringProcessing.NormalDataToMetaData(File.ReadAllText(filePath)) is T temp)
                    {
                        return (true, temp);
                    }
                }
                else
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(filePath);
                        using (MemoryStream memoryStream = new MemoryStream(bytes))
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            return (true, (T)binaryFormatter.Deserialize(memoryStream));
                        }
                    }
                    catch { return (false, new T()); }
                }
            }
            return (false, new T());
        }

        /// <summary>
        /// 依据绝对路径，直接读取文件
        /// </summary>
        public static (bool, T) DeserializeObject<T>(DataTypes type, string filePath) where T : class, new()
        {
            if (string.IsNullOrEmpty(filePath)) { return (false, new T()); }

            if (type == DataTypes.PublicAutoData)
            {
                if (StringProcessing.NormalDataToSong(File.ReadAllText(filePath)) is T temp)
                {
                    return (true, temp);
                }
            }
            else if (type == DataTypes.PublicVisualData)
            {
                if (StringProcessing.NormalDataToMetaData(File.ReadAllText(filePath)) is T temp)
                {
                    return (true, temp);
                }
            }
            else
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    using (MemoryStream memoryStream = new MemoryStream(bytes))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        return (true, (T)binaryFormatter.Deserialize(memoryStream));
                    }
                }
                catch { return (false, new T()); }
            }
            return (false, new T());
        }

        /// <summary>
        /// 选择并读取Txt文件
        /// </summary>
        /// <returns>( 是否成功 ， 文件名 ， 文本内容 )</returns>
        public static (bool, string, string) ReadTxtFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择键盘谱子";
            openFileDialog.Filter = "键盘谱子 (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                string result = File.ReadAllText(filePath);

                return (true, Path.GetFileNameWithoutExtension(filePath), result);
            }

            return (false, string.Empty, string.Empty);
        }

        /// <summary>
        /// 依据数据类型，获取所有该类型数据文件的文件名、绝对路径
        /// </summary>
        /// <param name="target">数据类型</param>
        /// <returns>List fileName + List filePath</returns>
        public static Tuple<List<string>, List<string>> GetFilesInfo(DataTypes target)
        {
            string folderPath = ParseFloderByDataType(target);

            List<string> fileNames = new List<string>();
            List<string> filePaths = new List<string>();

            try
            {
                string[] files = Directory.GetFiles(folderPath);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string fullPath = Path.GetFullPath(file);

                    fileNames.Add(fileName);
                    filePaths.Add(fullPath);
                }
            }
            catch { }

            return Tuple.Create(fileNames, filePaths);
        }

        /// <summary>
        /// 依据数据类型,获取数据所在文件夹路径
        /// </summary>
        /// <param name="target">数据类型</param>
        public static string ParseFloderByDataType(DataTypes target)
        {
            string result = string.Empty;
            switch (target)
            {
                case DataTypes.PublicAutoData:
                    result = PublicAutoData;
                    break;
                case DataTypes.PublicVisualData:
                    result = PublicVisualData;
                    break;
                case DataTypes.PrivateAutoData:
                    result = PrivateAutoData;
                    break;
                case DataTypes.PrivateVisualData:
                    result = PrivateVisualData;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 依据数据类型，获取OpenFileDialog的筛选器文本
        /// </summary>
        /// <param name="target">数据类型</param>
        public static string ParseFilterByDataType(DataTypes target)
        {
            string result = string.Empty;
            switch (target)
            {
                case DataTypes.PublicAutoData:
                    result = "文本分析数据 - 公开协议 (*.txt)|*.txt";
                    break;
                case DataTypes.PublicVisualData:
                    result = "简谱分析数据 - 公开协议 (*.txt)|*.txt";
                    break;
                case DataTypes.PrivateAutoData:
                    result = "文本分析数据 - 私有协议 (*.bin)|*.bin";
                    break;
                case DataTypes.PrivateVisualData:
                    result = "简谱分析数据 - 私有协议 (*.bin)|*.bin";
                    break;
            }
            return result;
        }
    }
}
