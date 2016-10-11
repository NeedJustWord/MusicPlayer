using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Converters
{
    class BitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource bitmapSource = (BitmapSource)value;
            if (parameter.ToString() == "Width") return bitmapSource.PixelWidth;
            return bitmapSource.PixelHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
