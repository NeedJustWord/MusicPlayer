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
            return File.Exists(ConfigPath) ? SerializationHelper.DeserializeObjectFromFile<ConfigInfo>(ConfigPath, SerializationFormatterType.Xml) : new ConfigInfo();
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
