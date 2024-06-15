using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using WindowsInput.Native;

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

        public static DataTypes Type = DataTypes.PublicStruct;

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
            set
            {
                _isOnPlaying = value;
            }
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
                    TxtAnalizeVisual.WhiteColor(Position, Brushes.White);
                    _position = value;
                }
                //同步坐标

                double temprate = (Position == notes.Count - 1 ? 1f : (double)Position / (notes.Count - 1) - 0.006f);

                if (TxtAnalizeVisual.Instance != null)
                {
                    TxtAnalizeVisual.Instance.ProcessShow.SetValue(temprate);
                    //同步进度比率

                    TxtAnalizeVisual.Instance.NotesBox.ScrollToHorizontalOffset(Position < notes.Count - 11 ? (Position - 11) * 60f : (Position + 1) * 60);
                    //同步文本音符可视区
                }

                if (!TxtAnalizeVisual.IsOnSlider) { TxtAnalizeVisual.Slider.Value = temprate; }
                //同步进度拖条


                if (Position < notes.Count) { TxtAnalizeVisual.Instance?.NewInfo(notes[Position]); }
                if (!IsOnPlaying) { TxtAnalizeVisual.WhiteColor(Position, Brushes.Red); }
                //同步控件提示色
            }
        }

        public async void Start()
        {
            if (IsOnPlaying) { return; }
            if (EditArea.PageType == PageTypes.TxtAnalize && HotKeySet.IsAutoAttentive)
            {
                TxtAnalizeVisual.ExpendBox();
            }
            IsOnPlaying = true;
            IsStop = false;
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

                    UpdateGameVisual(note.Key, note.Span);

                    await Task.Delay(note.Span);
                }
                else if (notes[i] is Chord chord)
                {
                    if (Model == PlayModel.Preview) { chord.Preview(); }
                    else if (Model == PlayModel.Auto) { chord.Play(); }
                    TxtAnalizeVisual.ColorChange(Position, chord.Chords.Last().Span, chord.GetContent());

                    int temptime = chord.Chords.Last().Span;
                    foreach (Note item in chord.Chords)
                    {
                        UpdateGameVisual(item.Key, temptime);
                    }

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
            IsStop = false;
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
        }

        /// <summary>
        /// 从当前索引处插入一段新的解析结果，注意不会保留该索引处原有的内容
        /// </summary>
        /// <param name="target"></param>
        public static Song AddParagraph(Song oldOne, string targetTxt)
        {
            Song temp1 = new Song();
            Song temp2 = new Song();
            Song temp3;
            temp3 = TxtAnalizeVisual.IsNormalInput ? StringProcessing.NormalDataToSong(targetTxt) : StringProcessing.SongParse(targetTxt);
            for (int i = 0; i < oldOne.Position; i++)
            {
                temp1.notes.Add(oldOne.notes[i]);
            }
            for (int i = oldOne.Position + 1; i < oldOne.notes.Count; i++)
            {
                temp2.notes.Add(oldOne.notes[i]);
            }

            Song result = temp1 + temp3 + temp2;

            return result;
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

        private void UpdateGameVisual(VirtualKeyCode key, int time)
        {
            TxtAnalizeVisual.PopupX.UpdateAnimation(key, time);
        }
    }
}
