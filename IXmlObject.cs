using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// 【接口】采用XML格式，完成对示例对象的读写操作 
    /// </summary>
    internal interface IXmlObject
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
        /// IXmlObject接口下的任意类型的实例对象可用此方法存储
        /// </summary>
        /// <typeparam name="T">实现IXmlObject接口的类型</typeparam>
        public static bool SaveObject<T>(T target, DataTypes type) where T : IXmlObject
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string? filePath = SelectFilePath(type);
            if (filePath == null) { return false; }
            try
            {
                using (TextWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, target);
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// IXmlObject接口下可使用此方法读取实例对象
        /// </summary>
        /// <typeparam name="T">IXmlObject下的类型</typeparam>
        public static T ReadObject<T>(DataTypes type) where T : IXmlObject, new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            string? filePath = SelectFilePath(type);
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

        public static string? SelectFilePath(DataTypes type)
        {
            string? filePath = null;

            switch (type)
            {
                case DataTypes.Simple:
                    filePath = SimpleStructData;
                    break;
                case DataTypes.Complex_NMN:
                    filePath = ComplexData_NMN;
                    break;
            }

            return filePath;
        }
    }
}
