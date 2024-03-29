using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicPlayer.Controls
{
    public class RollingElement
    {
        private static Dictionary<DependencyObject, DoubleAnimation> dictAnimations = new Dictionary<DependencyObject, DoubleAnimation>();

        #region 是否有跑马灯效果
        public static bool GetIsRolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRollingProperty);
        }

        public static void SetIsRolling(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRollingProperty, value);
        }

        public static readonly DependencyProperty IsRollingProperty =
            DependencyProperty.RegisterAttached("IsRolling", typeof(bool), typeof(RollingElement), new PropertyMetadata(false, IsRollingPropertyChangedCallback));

        private static void IsRollingPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                var parent = (FrameworkElement)fe.Parent;
                dictAnimations.Remove(fe);
                fe.SizeChanged -= Element_SizeChanged;
                parent.SizeChanged -= Parent_SizeChanged;

                if ((bool)e.NewValue == false) return;

                dictAnimations.Add(fe, new DoubleAnimation
                {
                    By = GetByValue(GetStep(fe)),
                    From = 0,
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                });
                fe.SizeChanged += Element_SizeChanged;
                parent.SizeChanged += Parent_SizeChanged;
            }
        }

        private static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetRolling((FrameworkElement)sender);
        }

        private static void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DependencyObject child;
            var d = (DependencyObject)sender;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                child = VisualTreeHelper.GetChild(d, i);
                if (dictAnimations.ContainsKey(child))
                {
                    SetRolling((FrameworkElement)child);
                }
            }
        }

        private static void SetRolling(FrameworkElement fe)
        {
            DoubleAnimation animation;
            var to = ((FrameworkElement)fe.Parent).ActualWidth - fe.ActualWidth;
            if (to >= 0)
            {
                animation = null;
            }
            else
            {
                animation = dictAnimations[fe];
                animation.To = to;
                animation.Duration = TimeSpan.FromSeconds(to / animation.By.Value);
            }

            fe.BeginAnimation(Canvas.LeftProperty, animation);
        }
        #endregion

        #region 步长
        public static double GetStep(DependencyObject obj)
        {
            return (double)obj.GetValue(StepProperty);
        }

        public static void SetStep(DependencyObject obj, double value)
        {
            obj.SetValue(StepProperty, value);
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.RegisterAttached("Step", typeof(double), typeof(RollingElement), new PropertyMetadata(default(double), StepPropertyChangedCallback));

        private static void StepPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (dictAnimations.TryGetValue(d, out var animation))
            {
                animation.By = GetByValue((double)e.NewValue);
                SetRolling((FrameworkElement)d);
            }
        }

        private static double GetByValue(double by) => by == 0 ? -15 : (by > 0 ? -by : by);
        #endregion
    }
}
