using System;

namespace MusicPlayer.Helpers
{
    class RandomHelper
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

        public static int GetNextIndex(int count, int notNumber)
        {
            var index = Random.Next(0, count);
            while (index == notNumber)
            {
                index = Random.Next(0, count);
            }
            return index;
        }
    }
}
