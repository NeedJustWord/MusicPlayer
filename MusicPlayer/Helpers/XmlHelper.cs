using System;
using System.Collections.Generic;
using System.IO;
using MusicPlayer.Models;

namespace MusicPlayer.Helpers
{
    public static class XmlHelper
    {
        private static readonly string ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
        private static readonly string MusicListPath = AppDomain.CurrentDomain.BaseDirectory + "MusicList.xml";

        public static ConfigInfo GetConfigInfo()
        {
            if (!File.Exists(ConfigPath))
                return new ConfigInfo
                {
                    IsMuted = false,
                    Volume = 0.6,
                    PlayMode = PlayMode.SequentialPlay,
                    Opacity = 1,
                    PlayStatus = PlayStatus.Pause,
                    MusicName = "多米音乐 欢迎您",
                    Singer = "",
                    Position = TimeSpan.FromSeconds(0),
                    TimeLength = "00:00:00",
                    MusicTime = "00:00|00:00"
                };

            var result = SerializationHelper.DeserializeObjectFromFile<ConfigInfo>(ConfigPath, SerializationFormatterType.Xml);
            result.PlayStatus = PlayStatus.Pause;
            result.MusicName = "多米音乐 欢迎您";
            result.Singer = "";
            result.Position = TimeSpan.FromSeconds(0);
            result.TimeLength = "00:00:00";
            result.MusicTime = "00:00|00:00";
            return result;
        }

        public static void SaveConfigInfo(ConfigInfo configInfo)
        {
            SerializationHelper.SerializeObjectToFile(configInfo, SerializationFormatterType.Xml, ConfigPath);
        }

        public static List<MusicInfo> GetMusicInfoList()
        {
            if (!File.Exists(MusicListPath))
                return new List<MusicInfo>();

            var result = SerializationHelper.DeserializeObjectFromFile<List<MusicInfo>>(MusicListPath, SerializationFormatterType.Xml);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].RowNum = i + 1;
            }
            return result;
        }

        public static void SaveMusicInfoList(List<MusicInfo> musicInfoList)
        {
            SerializationHelper.SerializeObjectToFile(musicInfoList, SerializationFormatterType.Xml, MusicListPath);
        }
    }
}
