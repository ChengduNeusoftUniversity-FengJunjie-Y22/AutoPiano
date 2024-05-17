using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPiano
{
    /// <summary>
    /// 【抽象类】二音乐器
    /// </summary>
    internal abstract class Instrument_DoubleKey : IMusicalInstrument
    {
        public abstract InstrumentTypes InstrumentType { get; }

    }
}
