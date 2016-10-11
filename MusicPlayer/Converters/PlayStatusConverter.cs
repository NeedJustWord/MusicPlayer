using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Converters
{
    class PlayStatusToImageConverter : IValueConverter
    {
        private readonly string[] _btnImages = { "/Images/Play.png", "/Images/Pause.png" };
        private readonly string[] _gridViewImages = { "/Images/PlayStatus.png", "/Images/PauseStatus.png" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] images = parameter.ToString() == "Button" ? _btnImages : _gridViewImages;

            PlayStatus status = (PlayStatus)value;
            ImageSource result = null;
            switch (status)
            {
                case PlayStatus.Play:
                    result = new BitmapImage(new Uri(images[0], UriKind.Relative));
                    break;
                case PlayStatus.Pause:
                    result = new BitmapImage(new Uri(images[1], UriKind.Relative));
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class PlayStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PlayStatus status = (PlayStatus)value;
            Visibility visibility = (Visibility)parameter;

            if (status == PlayStatus.Normal) return visibility;
            if (visibility == Visibility.Visible) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
