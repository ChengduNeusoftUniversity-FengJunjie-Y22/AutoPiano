using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPiano
{
    internal class Chord : XmlObject
    {
        public Chord() { }

        public List<Note> Chords = new List<Note>();//和弦=若干音符对象的合集

        //两种初始化和弦对象的方案
        public Chord(string chord, int span)
        {
            for (int i = 1; i < chord.Length - 1; i++)
            {
                if (i == chord.Length - 2)
                {
                    Chords.Add(new Note(chord[i], span));
                    break;
                }
                Chords.Add(new Note(chord[i], 0));
            }
        }
        public Chord(List<Note> notes, int span)
        {
            for (int i = 0; i < notes.Count; i++)
            {
                if (i == notes.Count - 1)
                {
                    Chords.Add(new Note(notes[i].Key, span));
                    break;
                }
                Chords.Add(new Note(notes[i].Key, 0));
            }
        }

        public void NewSpan(int target)
        {
            Chords.Last().Span = target;
        }

        public void Preview()
        {
            foreach (Note note in Chords)
            {
                note.Preview();
            }
        }
        public void Play()
        {
            foreach (Note note in Chords)
            {
                note.Play();
            }
        }
        public string GetContent()
        {
            string result = string.Empty;
            foreach (Note note in Chords)
            {
                result += note.GetContent();
            }
            return result;
        }
    }
}
