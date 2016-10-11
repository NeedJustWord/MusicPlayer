using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace MusicPlayer.Converters
{
    class SubStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SubString(value.ToString(), 8);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string SubString(string str, int maxLength)
        {
            //todo：截取8个字节的字符串
            return str != null && Encoding.Default.GetByteCount(str) > maxLength ? str.Substring(0, maxLength / 2) + "..." : str;
        }
    }
}
