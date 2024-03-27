using System.Windows;

namespace MusicPlayer.Controls
{
    public class OffsetElement
    {
        #region 是否偏移
        public static bool GetIsOffset(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOffsetProperty);
        }

        public static void SetIsOffset(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOffsetProperty, value);
        }

        public static readonly DependencyProperty IsOffsetProperty =
            DependencyProperty.RegisterAttached("IsOffset", typeof(bool), typeof(OffsetElement), new PropertyMetadata(false));
        #endregion

        #region X轴偏移量
        public static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }

        public static void SetX(DependencyObject obj, double value)
        {
            obj.SetValue(XProperty, value);
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.RegisterAttached("X", typeof(double), typeof(OffsetElement), new PropertyMetadata(double.NaN));
        #endregion

        #region Y轴偏移量
        public static double GetY(DependencyObject obj)
        {
            return (double)obj.GetValue(YProperty);
        }

        public static void SetY(DependencyObject obj, double value)
        {
            obj.SetValue(YProperty, value);
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.RegisterAttached("Y", typeof(double), typeof(OffsetElement), new PropertyMetadata(double.NaN));
        #endregion
    }
}
