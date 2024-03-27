using System;
using MusicPlayer.Helpers;
using MusicPlayer.Models;

namespace MusicPlayer
{
    class GlobalInfo
    {
        public static readonly string NextMusicToken = "NextMusic";
        public static readonly string UpdateInfoToken = "UpdateInfo";
        public static readonly string UpdatePlayProgressToken = "UpdatePlayProgress";
        public static readonly string[] PlayModeTexts = { "顺序", "循环", "随机", "单曲" };
        public static readonly Func<MusicInfo, string>[] OrderModeFuncs =
        {
            info => info.MusicName,
            info => info.Singer,
            info => info.PlayTimes.ToString("D8"),
            info => info.AddTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff"),
        };
        public static readonly ConfigInfo ConfigInfo = XmlHelper.GetConfigInfo();
    }

    public enum PlayStatus
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        Normal,
        /// <summary>
        /// 播放状态
        /// </summary>
        Play,
        /// <summary>
        /// 暂停状态
        /// </summary>
        Pause
    }

    public enum PlayMode
    {
        /// <summary>
        /// 顺序播放
        /// </summary>
        SequentialPlay = 0,
        /// <summary>
        /// 循环播放
        /// </summary>
        LoopPlay = 1,
        /// <summary>
        /// 随机播放
        /// </summary>
        RandomPlay = 2,
        /// <summary>
        /// 单曲循环
        /// </summary>
        SinglePlay = 3
    }

    public enum OrderMode
    {
        /// <summary>
        /// 按歌名排序
        /// </summary>
        MusicName = 0,
        /// <summary>
        /// 按歌手排序
        /// </summary>
        Singer = 1,
        /// <summary>
        /// 按播放次数排序
        /// </summary>
        PlayTimes = 2,
        /// <summary>
        /// 按添加时间排序
        /// </summary>
        AddTime = 3
    }

    public enum Sort
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc,
        /// <summary>
        /// 降序
        /// </summary>
        Desc
    }
}
