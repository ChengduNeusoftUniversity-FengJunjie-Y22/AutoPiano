using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPiano
{
    public interface IMusicMethod
    {
        int StartTime { get; set; }
        int Span { get; set; }
        void Preview();
        List<string> GetStringNodes();
    }
}
