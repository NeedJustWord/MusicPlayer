using System;
using GalaSoft.MvvmLight.Messaging;

namespace MusicPlayer.Helpers
{
    public static class MessengerHelper
    {
        public static void Register<T>(object obj, string token, Action<T> action)
        {
            Messenger.Default.Register(obj, token, action);
        }

        public static void Send<T>(string token, T message)
        {
            Messenger.Default.Send(message, token);
        }
    }
}
