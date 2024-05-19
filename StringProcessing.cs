using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace AutoPiano
{
    /// <summary>
    /// 【字符工具类】处理一切有关于字符的内容
    /// </summary>
    internal static class StringProcessing
    {
        /// <summary>
        /// 【TXT谱子】默认从这里被选择
        /// </summary>
        public static string DefaultTxtPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TXT");

        /// <summary>
        /// 检查文本信息文件夹是否完备
        /// </summary>
        public static void CheckTxtFloder()
        {
            if (!System.IO.Directory.Exists(DefaultTxtPath))
            {
                System.IO.Directory.CreateDirectory(DefaultTxtPath);
            }
        }

        public static async Task<Tuple<Song, string>> SelectThenAnalize()
        {
            Song song = new Song();
            string name = "?";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = DefaultTxtPath;
            openFileDialog.Filter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                string fileContent = File.ReadAllText(selectedFilePath);

                song = await SongParse(fileContent);
                name = System.IO.Path.GetFileNameWithoutExtension(selectedFilePath);
            }

            return Tuple.Create(song, name);
        }

        #region 文本谱解析工具
        /// <summary>
        /// 筛选条件: 单音符、和弦、间断都将被囊括(用于初步筛选)
        /// </summary>
        public static string target1 = @"\([A-Z]+\)| |[A-Z]|[\[A-Z()\]]+";

        /// <summary>
        /// 筛选条件: 在连弹中只包含单个音符、和弦(用于递归解决连弹问题)
        /// </summary>
        public static string target2 = @"\([A-Z]+\)|[A-Z]";

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

        public static async Task<Song> SongParse(string text)//将数据解析至Page1的song对象
        {
            Song result = new Song();
            await Task.Run(() =>
            {
                RecursivParse(text, false, 0, result);
            });
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
        #endregion
    }
}
