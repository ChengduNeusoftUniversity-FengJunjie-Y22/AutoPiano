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
    Simple,
    Complex_NMN,
    PublicStruct
}

namespace AutoPiano
{
    [Serializable]
    /// <summary>
    /// 【抽象类】采用XML格式，完成对实例对象的读写操作 
    /// </summary>
    public abstract class BinaryObject : AudioBasic
    {
        /// <summary>
        /// 【Operate+Time】结构的歌曲数据,结构简单，泛用性极高，适用于自动演奏，占用性能最少
        /// </summary>
        public static string SimpleStructData = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data_Simple");

        /// <summary>
        /// 【UIElement】支持的可视化歌曲数据，结构复杂，适合对文字谱效果不佳的曲子打简谱，预览时，占用性能多一些
        /// </summary>
        public static string ComplexData_NMN = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data_Complex_NMN");

        /// <summary>
        /// 检查数据存储文件夹是否完备
        /// </summary>
        public static void CheckDataFloder()
        {
            if (!System.IO.Directory.Exists(SimpleStructData))
            {
                System.IO.Directory.CreateDirectory(SimpleStructData);
            }
            if (!System.IO.Directory.Exists(ComplexData_NMN))
            {
                System.IO.Directory.CreateDirectory(ComplexData_NMN);
            }
        }

        /// <summary>
        /// 反射并根据名字创建绝对路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string? SelectFilePath<T>(DataTypes? type) where T : BinaryObject
        {
            FieldInfo? fieldInfo = typeof(T).GetField("Type", BindingFlags.Static | BindingFlags.Public);

            if (fieldInfo != null)
            {
                DataTypes reuslt = (type == null ? (DataTypes)fieldInfo.GetValue(null) : (DataTypes)type);
                string? filePath = null;

                OpenFileDialog folderBrowser = new OpenFileDialog();
                folderBrowser.Multiselect = false;
                folderBrowser.Filter = "Bin files (*.bin)|*.bin";

                switch (reuslt)
                {
                    case DataTypes.Simple:
                        folderBrowser.InitialDirectory = SimpleStructData;
                        if (folderBrowser.ShowDialog() == true)
                        {
                            filePath = folderBrowser.FileName;
                        }
                        break;
                    case DataTypes.Complex_NMN:
                        folderBrowser.InitialDirectory = ComplexData_NMN;
                        if (folderBrowser.ShowDialog() == true)
                        {
                            filePath = folderBrowser.FileName;
                        }
                        break;
                    case DataTypes.PublicStruct:
                        folderBrowser.InitialDirectory = StringProcessing.NormalTypeSongData;
                        if (folderBrowser.ShowDialog() == true)
                        {
                            filePath = folderBrowser.FileName;
                        }
                        break;
                }
                return filePath;
            }
            return null;
        }

        /// <summary>
        /// 序列化存储对象
        /// </summary>
        public static bool SerializeObject<T>(T song, DataTypes type, string name) where T : BinaryObject, new()
        {
            string? filePath = null;
            switch (type)
            {
                case DataTypes.Simple:
                    filePath = System.IO.Path.Combine(SimpleStructData, name + ".bin");
                    break;
                case DataTypes.Complex_NMN:
                    System.IO.Path.Combine(ComplexData_NMN, name + ".bin");
                    break;
            }

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();

                    binaryFormatter.Serialize(memoryStream, song);

                    File.WriteAllBytes(filePath, memoryStream.ToArray());
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// 反序列化存储对象
        /// </summary>
        public static (bool, T?) DeserializeObject<T>(DataTypes? type) where T : BinaryObject, new()
        {
            string? filePath = SelectFilePath<T>(type);
            if (filePath == null) { return (false, null); }
            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    return (true, (T)binaryFormatter.Deserialize(memoryStream));
                }
            }
            catch { return (false, null); }
        }
    }
}
