﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;
using WindowsInput;

namespace AutoPiano
{
    [Serializable]
    public class Note : AudioBasic, IMusicMethod
    {
        public Note() { }

        public int StartTime { get; set; } = 0;
        public int Span { get; set; } = 0;

        public VirtualKeyCode Key { get; set; }

        /// <param name="code">操作码</param>
        /// <param name="span">时值</param>
        public Note(VirtualKeyCode code, int span)
        {
            Key = code;
            Span = span;
        }

        /// <param name="value">操作字符</param>
        /// <param name="span">时值</param>
        public Note(char value, int span)
        {
            Key = GetKeyCode(value);
            Span = span;
        }

        public void NewSpan(int target)
        {
            Span = target;
        }

        public void Preview()
        {
            PlayWithKeyCode(Key);
        }

        public void Play()
        {
            Simulator.Keyboard.KeyDown(Key);
            Simulator.Keyboard.KeyUp(Key);
        }

        public string GetContent()
        {
            if (Span == 0)
            {
                return GetKeyChar(Key).ToString();
            }
            else
            {
                return GetKeyChar(Key).ToString() + " + " + Span;
            }
        }

        public string GetContentWithOutTime()
        {
            return GetKeyChar(Key).ToString();
        }

        public List<string> GetStringNodes()
        {
            List<string> nodes = new List<string>() { GetContentWithOutTime() };
            return nodes;
        }
    }
}
