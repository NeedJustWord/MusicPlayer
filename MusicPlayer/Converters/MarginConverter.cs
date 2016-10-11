using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MusicPlayer.Converters
{
    class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                switch (parameter.ToString())
                {
                    case "Left":
                        return new Thickness(3, 0, 0, 0);
                    case "Right":
                        return new Thickness(0, 0, 3, 0);
                    case "Up":
                        return new Thickness(0, 3, 0, 0);
                    case "Down":
                        return new Thickness(0, 0, 0, 3);
                }
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
