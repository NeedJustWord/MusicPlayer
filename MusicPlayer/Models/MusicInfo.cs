using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace MusicPlayer.Models
{
    public class MusicInfo : ViewModelBase
    {
        private int _rowNum;
        [XmlIgnore]
        public int RowNum
        {
            get { return _rowNum; }
            set { Set("RowNum", ref _rowNum, value); }
        }

        [XmlIgnore]
        public bool IsSelected { get; set; }

        public string FilePath { get; set; }

        public string MusicName { get; set; }

        public string Singer { get; set; }

        public string Album { get; set; }

        public string Rate { get; set; }

        public string TimeLength { get; set; }

        [XmlIgnore]
        public TimeSpan Position { get; set; }

        public DateTime AddTime { get; set; }

        public int PlayTimes { get; set; }

        private PlayStatus _playStatus;
        public PlayStatus PlayStatus
        {
            get { return _playStatus; }
            set { Set("PlayStatus", ref _playStatus, value); }
        }
    }

    //todo:暂定歌名和歌手相同就算重复
    class MusicInfoNoComparer : IEqualityComparer<MusicInfo>
    {
        public bool Equals(MusicInfo music1, MusicInfo music2)
        {
            if (music1 == null)
                return music2 == null;
            if (music2 == null)
                return false;
            return music1.MusicName == music2.MusicName && music1.Singer == music2.Singer;
        }

        public int GetHashCode(MusicInfo music)
        {
            if (music == null)
                return 0;
            return (music.MusicName + music.Singer).GetHashCode();
        }
    }
}
