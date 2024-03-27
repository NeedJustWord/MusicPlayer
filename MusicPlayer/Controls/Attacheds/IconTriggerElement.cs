using System.Windows;
using System.Windows.Media;

namespace MusicPlayer.Controls
{
    /// <summary>
    /// 图标触发器元素
    /// </summary>
    public class IconTriggerElement : IconElement
    {
        #region 鼠标移入图标背景
        public static Brush GetMouseOverBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBackgroundProperty);
        }

        public static void SetMouseOverBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBackgroundProperty, value);
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(IconTriggerElement), new PropertyMetadata(default(Brush)));
        #endregion

        #region 鼠标点击图标背景
        public static Brush GetPressedBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PressedBackgroundProperty);
        }

        public static void SetPressedBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(PressedBackgroundProperty, value);
        }

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.RegisterAttached("PressedBackground", typeof(Brush), typeof(IconTriggerElement), new PropertyMetadata(default(Brush)));
        #endregion
    }
}
