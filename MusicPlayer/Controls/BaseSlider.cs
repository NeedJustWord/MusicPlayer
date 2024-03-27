using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MusicPlayer.Controls
{
    public class BaseSlider : Slider
    {
        public static DependencyProperty IsShowBorderProperty = DependencyProperty.Register("IsShowBorder", typeof(bool), typeof(BaseSlider));

        public bool IsShowBorder
        {
            get { return (bool)GetValue(IsShowBorderProperty); }
            set { SetValue(IsShowBorderProperty, value); }
        }

        public static DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double),
            typeof(BaseSlider));

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static DependencyProperty ActualBorderBrushProperty = DependencyProperty.Register("ActualBorderBrush", typeof(Brush),
            typeof(BaseSlider));

        public Brush ActualBorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static DependencyProperty LeftBrushProperty = DependencyProperty.Register("LeftBrush", typeof(Brush),
            typeof(BaseSlider));

        public Brush LeftBrush
        {
            get { return (Brush)GetValue(LeftBrushProperty); }
            set { SetValue(LeftBrushProperty, value); }
        }

        public static DependencyProperty RightBrushProperty = DependencyProperty.Register("RightBrush", typeof(Brush),
            typeof(BaseSlider));

        public Brush RightBrush
        {
            get { return (Brush)GetValue(RightBrushProperty); }
            set { SetValue(RightBrushProperty, value); }
        }

        public static DependencyProperty IsPlayProgressProperty = DependencyProperty.Register("IsPlayProgress", typeof(bool),
            typeof(BaseSlider));

        public bool IsPlayProgress
        {
            get { return (bool)GetValue(IsPlayProgressProperty); }
            set { SetValue(IsPlayProgressProperty, value); }
        }

        /// <summary>
        /// IsPlayProgress为true时有效
        /// </summary>
        public bool IsMouseDown { get; private set; }

        private Track _track;
        private bool _isThumb;
        private bool _isMouseCauseValueChange;
        private double _oldValue;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _track = (Track)GetTemplateChild("PART_Track");

            if (_track != null && IsPlayProgress)
            {
                _track.DecreaseRepeatButton.Command = null;
                _track.IncreaseRepeatButton.Command = null;
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (IsPlayProgress)
            {
                if (!IsMouseDown && _isMouseCauseValueChange)
                {
                    _isMouseCauseValueChange = false;
                    base.OnValueChanged(oldValue, newValue);
                }
            }
            else
            {
                base.OnValueChanged(oldValue, newValue);
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsPlayProgress)
            {
                _oldValue = Value;
                _isMouseCauseValueChange = true;

                if (IsMoveToPointEnabled && _track?.Thumb != null && !_track.Thumb.IsMouseOver)
                {
                    _isThumb = false;
                    IsMouseDown = false;
                    Point pt = e.MouseDevice.GetPosition(_track);
                    Value = _track.ValueFromPoint(pt);
                }
                else
                {
                    _isThumb = true;
                    IsMouseDown = true;
                    base.OnPreviewMouseLeftButtonDown(e);
                }
            }
            else
            {
                base.OnPreviewMouseLeftButtonDown(e);
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsPlayProgress)
            {
                if (_isThumb)
                {
                    IsMouseDown = false;
                    OnValueChanged(_oldValue, Value);
                }
            }
            else
            {
                base.OnPreviewMouseLeftButtonUp(e);
            }
        }
    }
}
