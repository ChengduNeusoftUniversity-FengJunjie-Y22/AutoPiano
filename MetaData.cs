using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPiano
{
    internal class MetaData : XmlObject
    {
        public MetaData() { }

        /// <summary>
        /// 存放小节
        /// </summary>
        public List<ParagraphData> Data = new List<ParagraphData>();

        /// <summary>
        /// 复制小节对象的简化数据
        /// </summary>
        /// <param name="musicscore">目标对象</param>
        public void CopyDataFrom(NumberedMusicalNotation.MusicScore musicscore)
        {
            foreach (NumberedMusicalNotation.Paragraph paragraph in musicscore.Paragraphs)
            {
                ParagraphData paragraphData = new ParagraphData();
                paragraphData.CopyParagraphDataFrom(paragraph);
                Data.Add(paragraphData);
            }
        }

        /// <summary>
        /// 根据当前Data内存放的小节数据，生成可视化简谱对象
        /// </summary>
        /// <returns>NumberedMusicalNotation.MusicScore</returns>
        public NumberedMusicalNotation.MusicScore GetMusicScore()
        {
            NumberedMusicalNotation.MusicScore musicScore = new NumberedMusicalNotation.MusicScore();
            foreach (ParagraphData item in Data)
            {
                musicScore.Paragraphs.Add(item.GetParagraph());
            }
            return musicScore;
        }

        public class CoreData
        {
            public CoreData() { }
            public int Key = 1;//音阶Key                    默认为1        <-1  -7>   <1  7>    <8  14>
            public int Type = 1;//时值类型Type              默认为全音符   <1,2,4,8,16>
            public bool IsBlankStay = false;//是否为占位符  默认不是
            public bool IsFastColumn = false;//是否为滑音   默认不是

            public void Set(CoreSets sets, int value)
            {
                switch (sets)
                {
                    case CoreSets.Key: Key = value; break;
                    case CoreSets.Type: Type = value; break;
                }
            }
            public void Set(CoreSets sets, bool value)
            {
                switch (sets)
                {
                    case CoreSets.IsBlankStay: IsBlankStay = value; break;
                    case CoreSets.IsFastColumn: IsFastColumn = value; break;
                }
            }
            public void CopyDataFrom(NumberedMusicalNotation.Core core)//复制Core对象的数据
            {
                Set(CoreSets.Key, core.Key);
                Set(CoreSets.Type, core.Type);
                Set(CoreSets.IsBlankStay, core.IsBlankStay);
                Set(CoreSets.IsFastColumn, core.IsFastColumn);
            }
            public NumberedMusicalNotation.Core GetCore()//依据自身数据给出Core的实例对象
            {
                if (IsBlankStay)
                {
                    NumberedMusicalNotation.Core core = new NumberedMusicalNotation.Core();
                    core.IsBlankStay = true;
                    core.Type = Type;
                    core.CalculateCoreInfo();
                    return core;
                }
                else
                {
                    NumberedMusicalNotation.Core core = new NumberedMusicalNotation.Core();
                    core.Set(CoreSets.Key, Key);
                    core.Set(CoreSets.Type, Type);
                    core.Set(CoreSets.IsBlankStay, IsBlankStay);
                    core.Set(CoreSets.IsFastColumn, IsFastColumn);
                    core.CalculateCoreInfo();
                    return core;
                }
            }
        }

        public class TrackData
        {
            public TrackData() { }
            public List<CoreData> Data = new List<CoreData>();
            public void CopyTrackDataFrom(NumberedMusicalNotation.Track track)//复制Track对象的数据
            {
                foreach (var item in track.Cores.Children)
                {
                    if (item is NumberedMusicalNotation.Core)
                    {
                        NumberedMusicalNotation.Core core = (NumberedMusicalNotation.Core)item;
                        CoreData data = new CoreData();
                        data.CopyDataFrom(core);
                        Data.Add(data);
                    }
                }
            }
            public NumberedMusicalNotation.Track GetTrack()//依据自身数据给出Track的实例对象
            {
                NumberedMusicalNotation.Track track = new NumberedMusicalNotation.Track();
                foreach (CoreData item in Data)
                {
                    track.Cores.Children.Add(item.GetCore());
                }
                return track.GetGrid();
            }
        }

        public class ParagraphData
        {
            public ParagraphData() { }
            public List<TrackData> Data = new List<TrackData>();
            public void CopyParagraphDataFrom(NumberedMusicalNotation.Paragraph paragraph)//复制Paragraph对象的数据
            {
                foreach (var item in paragraph.Tracks.Children)
                {
                    if (item is NumberedMusicalNotation.Track)
                    {
                        NumberedMusicalNotation.Track track = (NumberedMusicalNotation.Track)item;
                        TrackData data = new TrackData();
                        data.CopyTrackDataFrom(track);
                        Data.Add(data);
                    }
                }
            }
            public NumberedMusicalNotation.Paragraph GetParagraph()//依据自身数据给出Paragraph的实例对象
            {
                NumberedMusicalNotation.Paragraph paragraph = new NumberedMusicalNotation.Paragraph();
                foreach (TrackData item in Data)
                {
                    paragraph.Tracks.Children.Add(item.GetTrack());
                }
                return paragraph.GetGrid();
            }
        }
    }
}
