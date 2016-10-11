using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicPlayer.Converters
{
    class PlayModeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PlayMode)value == (PlayMode)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
