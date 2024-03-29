using System.IO;
using MusicPlayer.Models;
using Shell32;

namespace MusicPlayer.Helpers
{
    public static class MusicInfoHelper
    {
        public static MusicInfo GetMusicInfo(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var dir = new ShellClass().NameSpace(Path.GetDirectoryName(filePath));
            var item = dir.ParseName(fileName);

            var musicInfo = new MusicInfo
            {
                MusicName = GetMusicName(dir, item, fileName),
                Singer = GetSinger(dir, item),
                TimeLength = GetTimeLength(dir, item),
                Album = GetAlbum(dir, item),
                Rate = GetRate(dir, item)
            };
            return musicInfo;
        }

        private static string GetMusicName(Folder dir, FolderItem item, string fileName)
        {
            var result = dir.GetDetailsOf(item, 21);
            return string.IsNullOrEmpty(result) ? fileName : result;
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
