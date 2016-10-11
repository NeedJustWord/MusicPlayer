using System.IO;
using MusicPlayer.Models;
using Shell32;

namespace MusicPlayer.Helpers
{
    public static class MusicInfoHelper
    {
        public static MusicInfo GetMusicInfo(string filePath)
        {
            ShellClass sh = new ShellClass();
            Folder dir = sh.NameSpace(Path.GetDirectoryName(filePath));
            FolderItem item = dir.ParseName(Path.GetFileName(filePath));

            MusicInfo musicInfo = new MusicInfo
            {
                MusicName = GetMusicName(dir, item),
                Singer = GetSinger(dir, item),
                TimeLength = GetTimeLength(dir, item),
                Album = GetAlbum(dir, item),
                Rate = GetRate(dir, item)
            };
            return musicInfo;
        }

        private static string GetMusicName(Folder dir, FolderItem item)
        {
            return dir.GetDetailsOf(item, 21);
        }

        private static string GetSinger(Folder dir, FolderItem item)
        {
            return dir.GetDetailsOf(item, 13);
        }

        private static string GetTimeLength(Folder dir, FolderItem item)
        {
            return dir.GetDetailsOf(item, 27);
        }

        private static string GetAlbum(Folder dir, FolderItem item)
        {
            return dir.GetDetailsOf(item, 14);
        }

        private static string GetRate(Folder dir, FolderItem item)
        {
            return dir.GetDetailsOf(item, 28);
        }
    }
}
