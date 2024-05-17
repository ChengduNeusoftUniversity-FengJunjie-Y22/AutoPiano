using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPiano
{
    internal interface IXmlObject
    {
        /// <summary>
        /// 【Operate+Time】结构的歌曲数据,结构简单，泛用性极高，适用于自动演奏，占用性能最少
        /// </summary>
        public static string SimpleStructData = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data_Simple");

        /// <summary>
        /// 【UIElement】支持的可视化歌曲数据，结构复杂，适合对文字谱效果不佳的曲子打简谱，预览时，占用性能多一些
        /// </summary>
        public static string NewVersionData = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data_Complex_NMN");

        /// <summary>
        /// 【TXT谱子】默认从这里被选择
        /// </summary>
        public static string DefaultTxtPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TXT");

        public static void CheckDataFloder()
        {

        }
    }
}
