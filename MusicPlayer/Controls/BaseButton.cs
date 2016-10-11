using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicPlayer.Controls
{
    public class BaseButton : Button
    {
        public static DependencyProperty NormalImageProperty = DependencyProperty.Register("NormalImage", typeof(ImageSource), typeof(BaseButton));

        public ImageSource NormalImage
        {
            get { return (ImageSource)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }

        public static DependencyProperty MouseOverImageProperty = DependencyProperty.Register("MouseOverImage", typeof(ImageSource), typeof(BaseButton));

        public ImageSource MouseOverImage
        {
            get { return (ImageSource)GetValue(MouseOverImageProperty); }
            set { SetValue(MouseOverImageProperty, value); }
        }

        public static DependencyProperty PressedImageProperty = DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(BaseButton));

        public ImageSource PressedImage
        {
            get { return (ImageSource)GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }

        public static DependencyProperty IsShiftingProperty = DependencyProperty.Register("IsShifting", typeof(bool), typeof(BaseButton));

        public bool IsShifting
        {
            get { return (bool)GetValue(IsShiftingProperty); }
            set { SetValue(IsShiftingProperty, value); }
        }

        public static DependencyProperty ShiftingOffsetXProperty = DependencyProperty.Register("ShiftingOffsetX", typeof(double), typeof(BaseButton));

        public double ShiftingOffsetX
        {
            get { return (double)GetValue(ShiftingOffsetXProperty); }
            set { SetValue(ShiftingOffsetXProperty, value); }
        }

        public static DependencyProperty ShiftingOffsetYProperty = DependencyProperty.Register("ShiftingOffsetY", typeof(double), typeof(BaseButton));

        public double ShiftingOffsetY
        {
            get { return (double)GetValue(ShiftingOffsetYProperty); }
            set { SetValue(ShiftingOffsetYProperty, value); }
        }
    }
}
