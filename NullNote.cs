using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace AutoPiano
{
    [Serializable]
    public class NullNote : BinaryObject
    {
        public NullNote() { }

        public NullNote(int timeValue)
        {
            Span = timeValue;
        }

        public VirtualKeyCode Key = VirtualKeyCode.VK_I;

        int _span = 0;

        public int Span
        {
            get { return _span; }
            set
            {
                if (value > -1)
                {
                    _span = value;
                    return;
                }
                _span = 0;
            }
        }

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
    }
}
