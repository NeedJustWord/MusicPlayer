﻿using System;
using System.Windows.Media;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace MusicPlayer.Models
{
    public class ConfigInfo : ViewModelBase
    {
        public double Top { get; set; } = 50;

        public double Left { get; set; } = 50;

        public double Width { get; set; } = 300;

        public double Height { get; set; } = 720;

        private double _opacity = 1;
        public double Opacity
        {
            get { return _opacity; }
            set { Set("Opacity", ref _opacity, value); }
        }

        private bool _isMuted;
        public bool IsMuted
        {
            get { return _isMuted; }
            set { Set("IsMuted", ref _isMuted, value); }
        }

        private double _volume = 0.6;
        public double Volume
        {
            get { return _volume; }
            set { Set("Volume", ref _volume, value); }
        }

        public OrderMode OrderMode { get; set; } = OrderMode.AddTime;

        public Sort Sort { get; set; } = Sort.Asc;

        private PlayMode _playMode;
        public PlayMode PlayMode
        {
            get { return _playMode; }
            set
            {
                _playMode = value;
                PlayModeText = GlobalInfo.PlayModeTexts[(int)value];
                PlayModeImage = GlobalInfo.PlayModeImages[(int)value];
            }
        }

        private string _playModeText;
        [XmlIgnore]
        public string PlayModeText
        {
            get { return _playModeText; }
            set { Set("PlayModeText", ref _playModeText, value); }
        }

        private ImageSource _playModeImage;
        [XmlIgnore]
        public ImageSource PlayModeImage
        {
            get { return _playModeImage; }
            set { Set("PlayModeImage", ref _playModeImage, value); }
        }

        private PlayStatus _playStatus;
        [XmlIgnore]
        public PlayStatus PlayStatus
        {
            get { return _playStatus; }
            set { Set("PlayStatus", ref _playStatus, value); }
        }

        private string _musicName;
        [XmlIgnore]
        public string MusicName
        {
            get { return _musicName; }
            set { Set("MusicName", ref _musicName, value); }
        }

        private string _singer;
        [XmlIgnore]
        public string Singer
        {
            get { return _singer; }
            set { Set("Singer", ref _singer, value); }
        }

        private TimeSpan _position;
        [XmlIgnore]
        public TimeSpan Position
        {
            get { return _position; }
            set { Set("Position", ref _position, value); }
        }

        private string _timeLength;
        [XmlIgnore]
        public string TimeLength
        {
            get { return _timeLength; }
            set { Set("TimeLength", ref _timeLength, value); }
        }

        private string _musicTime;
        [XmlIgnore]
        public string MusicTime
        {
            get { return _musicTime; }
            set { Set("MusicTime", ref _musicTime, value); }
        }
    }
}
