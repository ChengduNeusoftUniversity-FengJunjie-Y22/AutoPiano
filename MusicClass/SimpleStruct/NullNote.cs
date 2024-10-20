using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace AutoPiano
{
    [Serializable]
    public class NullNote : AudioBasic, IMusicMethod
    {
        public NullNote() { }

        public int StartTime { get; set; } = 0;
        public int Span { get; set; } = 0;

        public NullNote(int timeValue)
        {
            Span = timeValue;
        }

        public VirtualKeyCode Key = VirtualKeyCode.VK_I;

        public void NewSpan(int span)
        {
            Span = span;
        }

        public void Preview()
        {

        }

        public void Play()
        {

        }

        public string GetContent()
        {
            if (Span == 0)
            {
                return "0";
            }
            else
            {
                return "0" + " + " + Span;
            }
        }

        public string GetContentWithOutTime()
        {
            return "0";
        }

        public List<string> GetStringNodes()
        {
            return new List<string>();
        }
    }
}
