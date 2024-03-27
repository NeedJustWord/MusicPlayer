using System.Windows;
using System.Windows.Media;

namespace MusicPlayer.Controls
{
    public class MultiIconElement : IconElement
    {
        #region 图标1
        public static Geometry GetIcon1(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(Icon1Property);
        }

        public static void SetIcon1(DependencyObject obj, Geometry value)
        {
            obj.SetValue(Icon1Property, value);
        }

        public static readonly DependencyProperty Icon1Property =
            DependencyProperty.RegisterAttached("Icon1", typeof(Geometry), typeof(MultiIconElement), new PropertyMetadata(default(Geometry)));
        #endregion

        #region 图标颜色1
        public static Brush GetColor1(DependencyObject obj)
        {
            return (Brush)obj.GetValue(Color1Property);
        }

        public static void SetColor1(DependencyObject obj, Brush value)
        {
            obj.SetValue(Color1Property, value);
        }

        public static readonly DependencyProperty Color1Property =
            DependencyProperty.RegisterAttached("Color1", typeof(Brush), typeof(MultiIconElement), new PropertyMetadata(default(Brush)));
        #endregion

        #region 图标画笔宽度1
        public static double GetStrokeThickness1(DependencyObject obj)
        {
            return (double)obj.GetValue(StrokeThickness1Property);
        }

        public static void SetStrokeThickness1(DependencyObject obj, double value)
        {
            obj.SetValue(StrokeThickness1Property, value);
        }

        public static readonly DependencyProperty StrokeThickness1Property =
            DependencyProperty.RegisterAttached("StrokeThickness1", typeof(double), typeof(MultiIconElement), new PropertyMetadata(double.NaN));
        #endregion

        #region 图标边缘呈现方式1
        public static EdgeMode GetEdgeMode1(DependencyObject obj)
        {
            return (EdgeMode)obj.GetValue(EdgeMode1Property);
        }

        public static void SetEdgeMode1(DependencyObject obj, EdgeMode value)
        {
            obj.SetValue(EdgeMode1Property, value);
        }

        public static readonly DependencyProperty EdgeMode1Property =
            DependencyProperty.RegisterAttached("EdgeMode1", typeof(EdgeMode), typeof(MultiIconElement), new PropertyMetadata(EdgeMode.Aliased));
        #endregion
    }
}
