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
using System.Xml.Serialization;
using System.Xml.XPath;

public enum DataTypes
{
    Simple,
    Complex_NMN
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
        public static string? SelectFilePath<T>(string name) where T : BinaryObject
        {
            FieldInfo? fieldInfo = typeof(T).GetField("Type", BindingFlags.Static | BindingFlags.Public);

            if (fieldInfo != null)
            {
                DataTypes reuslt = (DataTypes)fieldInfo.GetValue(null);
                string? filePath = null;

                switch (reuslt)
                {
                    case DataTypes.Simple:
                        filePath = Path.Combine(SimpleStructData, name);
                        break;
                    case DataTypes.Complex_NMN:
                        filePath = Path.Combine(ComplexData_NMN, name);
                        break;
                }
                return filePath;
            }
            return null;
        }

        /// <summary>
        /// 序列化存储对象
        /// </summary>
        public static void SerializeObject<T>(T song, string name) where T : BinaryObject, new()
        {
            string? filePath = SelectFilePath<T>(name);
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();

                    binaryFormatter.Serialize(memoryStream, song);

                    File.WriteAllBytes(filePath, memoryStream.ToArray());
                }
            }
            catch { MessageBox.Show("存储过程发生意外！"); }
        }

        /// <summary>
        /// 反序列化存储对象
        /// </summary>
        public static T DeserializeObject<T>(string name) where T : BinaryObject, new()
        {
            string? filePath = SelectFilePath<T>(name);
            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    return (T)binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch { MessageBox.Show("读取过程发生意外！"); return new T(); }
        }
    }
}
