using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

public enum PlayModel
{
    Auto,
    Preview,
    None
}

namespace AutoPiano
{
    [Serializable]
    public class Song : BinaryObject
    {
        public string Name = "None";

        public static DataTypes Type = DataTypes.Simple;
        public PlayModel Model = PlayModel.None;

        public Song() { }

        /// <summary>
        /// 存放音符对象，可以是单音、和弦、占位符
        /// </summary>
        public List<object> notes = new List<object>();

        private bool _isOnPlaying = false;
        public bool IsOnPlaying
        {
            get { return _isOnPlaying; }
            set { _isOnPlaying = value; }
        }

        private bool _isStop = false;
        public bool IsStop
        {
            get { return _isStop; }
            set { _isStop = value; }
        }

        private int _position = 0;
        public int Position
        {
            get
            {
                if (_position < notes.Count)
                {
                    return _position;
                }
                return 0;
            }
            set
            {
                if (value < notes.Count && value >= 0)
                {
                    if (Model == PlayModel.Preview) { TxtAnalizeVisual.WhiteColor(_position, Brushes.White); }
                    _position = value;
                }
                else
                {
                    _position = 0;
                }
                if (Model == PlayModel.Preview)
                {
                    if (_position < notes.Count) { TxtAnalizeVisual.Instance?.NewInfo(notes[_position]); }
                    if (!IsOnPlaying) { TxtAnalizeVisual.WhiteColor(_position, Brushes.Red); }
                }
            }
        }

        public async void Start()
        {
            if (IsOnPlaying) return;
            IsOnPlaying = true;
            for (int i = Position; i < notes.Count; i++)
            {
                if (IsStop)
                {
                    IsStop = false;
                    IsOnPlaying = false;
                    Position--;
                    return;
                }
                if (notes[i] is Note note)
                {
                    if (Model == PlayModel.Preview) { note.Preview(); }
                    else if (Model == PlayModel.Auto) { note.Play(); }
                    TxtAnalizeVisual.ColorChange(Position, note.Span, note.GetContent());
                    await Task.Delay(note.Span);
                }
                else if (notes[i] is Chord chord)
                {
                    if (Model == PlayModel.Preview) { chord.Preview(); }
                    else if (Model == PlayModel.Auto) { chord.Play(); }
                    TxtAnalizeVisual.ColorChange(Position, chord.Chords.Last().Span, chord.GetContent());
                    await Task.Delay(chord.Chords.Last().Span);
                }
                else if (notes[i] is NullNote nunote)
                {
                    TxtAnalizeVisual.ColorChange(Position, nunote.Span, nunote.GetContent());
                    await Task.Delay(nunote.Span);
                }
                Position++;
            }
            IsOnPlaying = false;
            Position = 0;
        }
        public void Stop()
        {
            if (IsOnPlaying)
            {
                IsStop = true;
                Position = 0;
            }
        }
        public void Pause()
        {
            if (IsOnPlaying)
            {
                IsStop = true;
            }
            else
            {
                Start();
            }
        }

        /// <summary>
        /// 从当前索引处插入一段新的解析结果，注意不会保留该索引处原有的内容
        /// </summary>
        /// <param name="target"></param>
        public async void AddParagraph(string target)
        {
            Song temp1 = new Song();
            Song temp2 = new Song();
            Song temp3 = new Song();
            Song result = new Song();
            for (int i = 0; i < Position; i++)
            {
                temp1.notes.Add(notes[i]);
            }
            for (int i = Position + 1; i < notes.Count; i++)
            {
                temp2.notes.Add(notes[i]);
            }
            temp3 = await StringProcessing.SongParse(target);
            result = temp1 + temp3 + temp2;
            notes = result.notes;
        }

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

        public static Song Copy(Song target)
        {
            Song temp = new Song();
            temp.Name = target.Name;

            foreach (object s in target.notes)
            {
                temp.notes.Add(s);
            }

            return temp;
        }
    }
}
