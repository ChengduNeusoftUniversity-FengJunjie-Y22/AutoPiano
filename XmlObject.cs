using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.XPath;

public enum DataTypes
{
    Simple,
    Complex_NMN
}

namespace AutoPiano
{
    /// <summary>
    /// 【抽象类】采用XML格式，完成对实例对象的读写操作 
    /// </summary>
    public abstract class XmlObject : AudioBasic
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
        /// 存储实例对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="target">要存储的对象</param>
        /// <param name="type">数据类型</param>
        /// <param name="fileName">文件名</param>
        /// <returns>存储是否成功</returns>
        public static bool SaveObject<T>(T target, DataTypes type, string fileName) where T : XmlObject
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string? filePath = SelectFilePath(type, fileName);
            if (filePath == null) { return false; }
            try
            {
                using (TextWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, target);
                    writer.Flush(); // 刷新缓冲区，确保数据被写入文件
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 读取实例对象
        /// </summary>
        public static T ReadObject<T>(DataTypes type, string fileName) where T : XmlObject, new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string? filePath = SelectFilePath(type, fileName);
            if (filePath == null) { return new T(); }
            try
            {
                using (TextReader reader = new StreamReader(filePath))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch { return new T(); }
        }

        public static string? SelectFilePath(DataTypes type, string fileName)
        {
            string? filePath = null;

            switch (type)
            {
                case DataTypes.Simple:
                    filePath = Path.Combine(SimpleStructData, fileName);
                    break;
                case DataTypes.Complex_NMN:
                    filePath = Path.Combine(ComplexData_NMN, fileName);
                    break;
            }

            return filePath;
        }
    }
}
