using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoPiano
{
    [Serializable]
    public class Song : BinaryObject
    {
        public string Name = "None";

        public static DataTypes Type = DataTypes.Simple;

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
                if (value < notes.Count)
                {
                    _position = value;
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
                UpdateTxtVisual();
                if (notes[i] is Note note)
                {
                    note.Preview();
                    TxtAnalizeVisual.ColorChange(Position, note.Span, note.GetContent());
                    await Task.Delay(note.Span);
                }
                else if (notes[i] is Chord chord)
                {
                    chord.Preview();
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

        public void UpdateTxtVisual()
        {
            if (TxtAnalizeVisual.Instance != null)
            {
                TxtAnalizeVisual.Instance.SDValuePlay.Value = Position * 60;
            }
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
    }
}
