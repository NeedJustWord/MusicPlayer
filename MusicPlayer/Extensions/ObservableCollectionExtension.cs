using System;
using System.Collections.ObjectModel;

namespace MusicPlayer.Extensions
{
    static class ObservableCollectionExtension
    {
        public static void ForEach<T>(this ObservableCollection<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
