using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPiano
{
    internal class Song : XmlObject
    {
        public Song() { }

        /// <summary>
        /// 存放音符对象，可以是单音、和弦、占位符
        /// </summary>
        public List<object> notes = new List<object>();

        /// <summary>
        /// 实现曲子的衔接
        /// </summary>
        /// <param name="a">被衔接的曲子Song</param>
        /// <param name="b">新的曲段Song</param>
        /// <returns></returns>
        public static Song operator +(Song a, Song b)
        {
            foreach (object s in b.notes)
            {
                a.notes.Add(s);
            }
            return a;
        }

        /// <summary>
        /// 实现歌曲整体调速
        /// </summary>
        /// <param name="a">目标歌曲Song</param>
        /// <param name="b">缩放倍率double</param>
        /// <returns></returns>
        public static Song operator *(Song a, double b)
        {
            foreach (object s in a.notes)
            {
                if (s is Note)
                {
                    Note note = (Note)s;
                    note.Span = Convert.ToInt32(note.Span * b);
                }
                else if (s is Chord)
                {
                    Chord ch = (Chord)s;
                    int c = ch.Chords.Last().Span;
                    ch.NewSpan(Convert.ToInt32(c * b));
                }
                else if (s is NullNote)
                {
                    NullNote note = (NullNote)s;
                    note.Span = Convert.ToInt32(note.Span * b);
                }
            }
            return a;
        }
    }
}
