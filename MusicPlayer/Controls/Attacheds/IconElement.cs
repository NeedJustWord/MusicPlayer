using System.Windows;
using System.Windows.Media;

namespace MusicPlayer.Controls
{
    /// <summary>
    /// 图标元素
    /// </summary>
    public class IconElement
    {
        #region 图标
        public static Geometry GetIcon(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, Geometry value)
        {
            obj.SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(Geometry), typeof(IconElement), new PropertyMetadata(default(Geometry)));
        #endregion

        #region 图标图片
        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageProperty);
        }

        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(IconElement), new PropertyMetadata(default(ImageSource)));
        #endregion

        #region 图标颜色
        public static Brush GetColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ColorProperty);
        }

        public static void SetColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.RegisterAttached("Color", typeof(Brush), typeof(IconElement), new PropertyMetadata(default(Brush)));
        #endregion

        #region 图标宽度
        public static double GetWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject obj, double value)
        {
            obj.SetValue(WidthProperty, value);
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(double), typeof(IconElement), new PropertyMetadata(double.NaN));
        #endregion

        #region 图标高度
        public static double GetHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(HeightProperty);
        }

        public static void SetHeight(DependencyObject obj, double value)
        {
            obj.SetValue(HeightProperty, value);
        }

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached("Height", typeof(double), typeof(IconElement), new PropertyMetadata(double.NaN));
        #endregion

        #region 图标画笔宽度
        public static double GetStrokeThickness(DependencyObject obj)
        {
            return (double)obj.GetValue(StrokeThicknessProperty);
        }

        public static void SetStrokeThickness(DependencyObject obj, double value)
        {
            obj.SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.RegisterAttached("StrokeThickness", typeof(double), typeof(IconElement), new PropertyMetadata(double.NaN));
        #endregion

        #region 图标边缘呈现方式
        public static EdgeMode GetEdgeMode(DependencyObject obj)
        {
            return (EdgeMode)obj.GetValue(EdgeModeProperty);
        }

        public static void SetEdgeMode(DependencyObject obj, EdgeMode value)
        {
            obj.SetValue(EdgeModeProperty, value);
        }

        public static readonly DependencyProperty EdgeModeProperty =
            DependencyProperty.RegisterAttached("EdgeMode", typeof(EdgeMode), typeof(IconElement), new PropertyMetadata(EdgeMode.Aliased));
        #endregion
    }
}
