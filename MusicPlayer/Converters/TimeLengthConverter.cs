using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicPlayer.Converters
{
    class TimeLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] time = value.ToString().Split(':');
            return System.Convert.ToInt32(time[0]) * 3600 + System.Convert.ToInt32(time[1]) * 60 + System.Convert.ToInt32(time[2]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
