using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using WindowsInput.Native;

namespace AutoPiano
{
    /// <summary>
    /// 【字符工具类】处理一切有关于字符的内容
    /// </summary>
    internal static class StringProcessing
    {
        #region 文本谱解析工具
        private static int _blankspace = 187;
        /// <summary>
        /// 若两音符间无空格，则应用此时值
        /// </summary>
        public static int BlankSpace
        {
            get { return _blankspace; }
            set
            {
                if (value > 0 && value < 600)
                {
                    _blankspace = value;
                }
            }
        }

        private static int re_blankspace = 10;
        /// <summary>
        /// 回调模式下，时值增减的具体值
        /// </summary>
        public static int BlankSpace_Re
        {
            get { return re_blankspace; }
            set
            {
                if (value > 0 && value < 100)
                {
                    re_blankspace = value;
                }
            }
        }

        private static double _cutrate = 0.5;
        /// <summary>
        /// 当键盘谱空格过多时，时值的递增按此值削减
        /// </summary>
        public static int CutStartPosition
        {
            get { return _cutstartposition; }
            set
            {
                if (value >= 2)
                {
                    _cutstartposition = value;
                }
            }
        }

        private static int _cutstartposition = 3;
        /// <summary>
        /// 从(CutStartPosition+1)开始的空格，添加的间隔值将被削减
        /// </summary>
        public static double CutRate
        {
            get { return _cutrate; }
            set
            {
                if (value >= 0)
                {
                    _cutrate = value;
                }
            }
        }


        /// <summary>
        /// 筛选条件: 单音符、和弦、间断都将被囊括(用于初步筛选)
        /// </summary>
        private static string target1 = @"\([A-Z]+\)| |[A-Z]|[\[A-Z()\]]+";
        /// <summary>
        /// 筛选条件: 在连弹中只包含单个音符、和弦(用于递归解决连弹问题)
        /// </summary>
        private static string target2 = @"\([A-Z]+\)|[A-Z]";
        public static Song SongParse(string text)//将非通用数据解析，这里主要指B站指尖旋律的键盘谱格式
        {
            Song result = new Song();
            RecursivParse(text, false, 0, result);
            return result;
        }
        private static void RecursivParse(string text, bool isRecursiv, int value, Song Target)//解析按键谱
        {
            MatchCollection matches;//操作合集
            if (isRecursiv)
            {
                matches = Regex.Matches(text, target2);//若一个连弹操作被递归进来
            }
            else
            {
                matches = Regex.Matches(text, target1);//若是一般音符、和弦
            }

            for (int i = 0; i < matches.Count; i++)//遍历所有操作
            {
                if (isRecursiv)
                {
                    if (matches[i].Value.ToString()[0] >= 'A' && matches[i].Value.ToString()[0] <= 'Z')//若捕获到单个音符
                    {
                        if (i == matches.Count - 1)
                        {
                            Target.notes.Add(new Note(matches[i].Value.ToString()[0], value));
                        }
                        else
                        {
                            Target.notes.Add(new Note(matches[i].Value.ToString()[0], BlankSpace / 4));
                        }
                    }
                    else if (matches[i].Value.ToString()[0] == '(')//若获取到一个和弦
                    {
                        if (i == matches.Count - 1)
                        {
                            Target.notes.Add(new Chord(matches[i].Value.ToString(), value));
                        }
                        else
                        {
                            Target.notes.Add(new Chord(matches[i].Value.ToString(), BlankSpace / 4));
                        }
                    }
                }
                else
                {
                    int span = GetSpanFromSpaceNum(FindBlankSpaceNumber(matches, i));
                    if (matches[i].Value.ToString()[0] >= 'A' && matches[i].Value.ToString()[0] <= 'Z')//若捕获到单个音符
                    {
                        Target.notes.Add(new Note(matches[i].Value.ToString()[0], span));
                    }
                    else if (matches[i].Value.ToString()[0] == '(')//若获取到一个和弦
                    {
                        Target.notes.Add(new Chord(matches[i].Value.ToString(), span));
                    }
                    else if (matches[i].Value.ToString()[0] == '[')//连弹操作要进入递归
                    {
                        RecursivParse(matches[i].Value.ToString(), true, span, Target);
                    }
                }
            }
        }
        private static int GetSpanFromSpaceNum(int num)//获取时值
        {
            int result = 0;

            if (num == 0)
            {
                result = BlankSpace / 2;
            }
            else if (num > 0 && num < CutStartPosition)
            {
                result = BlankSpace * num;
            }
            else if (num >= CutStartPosition)//空格数量很多时，间隔的叠加效率将参考CutRate
            {
                result = (BlankSpace * (CutStartPosition - 1)) + (int)(BlankSpace * CutRate * (num - CutStartPosition + 1));
            }

            return result;
        }
        private static int FindBlankSpaceNumber(MatchCollection matches, int start)//获取跟随空格数量
        {
            int result = 0;

            for (int i = start + 1; i < matches.Count; i++)
            {
                if (matches[i].Value.ToString()[0] == ' ')
                {
                    result++;
                }
                else
                {
                    break;
                }
            }

            return result;
        }


        public static string SongToNormalData(Song target)//Song数据 => 通用格式数据
        {
            string result = string.Empty;

            result += target.Name.Replace(" ", string.Empty);
            result += " ";

            foreach (var item in target.notes)
            {
                if (item is Note note)
                {
                    result += note.GetContentWithOutTime();
                    result += " ";
                    result += note.Span;
                    result += " ";
                }
                else if (item is Chord chord)
                {
                    result += chord.GetContentWithOutTime();
                    result += " ";
                    result += chord.Chords.Last().Span;
                    result += " ";
                }
                else if (item is NullNote nullnote)
                {
                    result += "P ";
                    result += nullnote.Span;
                    result += " ";
                }
            }

            return result;
        }
        public static Song NormalDataToSong(string target)//通用格式数据 => Song数据 
        {
            if (target.Length == 0) return new Song();
            Song result = new Song();

            try
            {
                string[] parts = target.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                result.Name = parts[0];

                for (int i = 1; i <= parts.Length - 2; i += 2)
                {
                    int time = StringToInt(parts[i + 1]);
                    if (parts[i].Length == 1)
                    {
                        if (parts[i][0] == 'P')
                        {
                            result.notes.Add(new NullNote(time));
                        }
                        else
                        {
                            result.notes.Add(new Note(AudioBasic.CharToKeyCode[parts[i][0]], time));
                        }
                    }
                    else if (parts[i].Length > 1)
                    {
                        Chord chord = new Chord();
                        for (int k = 0; k < parts[i].Length; k++)
                        {
                            chord.Chords.Add(new Note(AudioBasic.CharToKeyCode[parts[i][k]], (k == parts[i].Length - 1 ? time : 0)));
                        }
                        result.notes.Add(chord);
                    }
                }
            }
            catch { return new Song(); }

            return result;
        }
        public static string MetaDataToNormalData(MetaData target)//MetaData数据 => 通用格式数据
        {
            StringBuilder result = new StringBuilder();

            foreach (MetaData.ParagraphData paraData in target.Data)
            {
                result.Append("{ ");
                foreach (MetaData.TrackData trackData in paraData.Data)
                {
                    result.Append("[ ");
                    foreach (MetaData.CoreData coreData in trackData.Data)
                    {
                        result.Append("( ");
                        result.Append(coreData.Key).Append(" ");
                        result.Append(coreData.Type).Append(" ");
                        result.Append(coreData.IsBlankStay).Append(" ");
                        result.Append(") ");
                    }
                    result.Append("] ");
                }
                result.Append("} ");
            }

            return result.ToString();
        }
        public static MetaData NormalDataToMetaData(string target)//通用格式数据 => MetaData数据
        {
            MetaData result = new MetaData();

            int Pindex = 0;
            int Tindex = 0;

            List<string> ParaString = ParagraphCatchFromMetaTxt(target);
            foreach (string str in ParaString)
            {
                result.Data.Add(new MetaData.ParagraphData());
                Tindex = 0;
                List<string> TrackString = TrackCatchFromMetaTxt(str);
                foreach (string str2 in TrackString)
                {
                    result.Data[Pindex].Data.Add(new MetaData.TrackData());
                    List<string> CoreString = CoreCatchFromMetaTxt(str2);
                    foreach (string str3 in CoreString)
                    {
                        string[] temp = str3.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        int keytemp = StringToInt(temp[0]);
                        int typetemp = StringToInt(temp[1]);
                        bool bltemp = StringToBool(temp[2]);
                        MetaData.CoreData coreData = new MetaData.CoreData();
                        coreData.Key = keytemp;
                        coreData.Type = typetemp;
                        coreData.IsBlankStay = bltemp;
                        result.Data[Pindex].Data[Tindex].Data.Add(coreData);
                    }
                    Tindex++;
                }
                Pindex++;
            }

            return result;
        }


        public static int StringToInt(string target)//string转整数
        {
            int result = 0;
            bool success = int.TryParse(target, out int temp);
            if (success)
            {
                result = temp;
            }
            return result;
        }
        public static bool StringToBool(string target)//string转bool
        {
            if (target == "True")
            {
                return true;
            }
            return false;
        }


        public static List<string> ParagraphCatchFromMetaTxt(string target)//捕获所有位于{}内的字符串
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\{(.*?)\}");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }
        public static List<string> TrackCatchFromMetaTxt(string target)//捕获所有位于[]内的字符串
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\[(.*?)\]");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }
        public static List<string> CoreCatchFromMetaTxt(string target)//捕获所有位于()内的字符串
        {
            List<string> result = new List<string>();

            Regex regex = new Regex(@"\((.*?)\)");
            MatchCollection matches = regex.Matches(target);

            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }

            return result;
        }
        #endregion
    }
}
