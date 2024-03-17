using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicPlayer.Controls
{
    public static class ToolTipService
    {
        private static DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));

        #region 当文本截取时显示ToolTip
        public static bool GetShowToolTipWhenTextTrimming(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowToolTipWhenTextTrimmingProperty);
        }

        public static void SetShowToolTipWhenTextTrimming(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowToolTipWhenTextTrimmingProperty, value);
        }

        public static readonly DependencyProperty ShowToolTipWhenTextTrimmingProperty =
            DependencyProperty.RegisterAttached("ShowToolTipWhenTextTrimming", typeof(bool), typeof(ToolTipService), new PropertyMetadata(false, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock tb)
            {
                tb.SizeChanged -= TextBlock_SizeChanged;
                descriptor.RemoveValueChanged(tb, TextBlock_TextChanged);

                if ((bool)e.NewValue == false) return;

                tb.SizeChanged += TextBlock_SizeChanged;
                descriptor.AddValueChanged(tb, TextBlock_TextChanged);
            }
        }

        private static void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetToolTip((TextBlock)sender);
        }

        private static void TextBlock_TextChanged(object sender, EventArgs e)
        {
            SetToolTip((TextBlock)sender);
        }

        private static void SetToolTip(TextBlock tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.ToolTip = null;
                return;
            }

            tb.ToolTip = IsTextTrimming(tb) ? tb.Text : null;
        }

        private static bool IsTextTrimming(TextBlock tb)
        {
            var typeface = new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
            var formattedText = new FormattedText(tb.Text, System.Threading.Thread.CurrentThread.CurrentCulture, tb.FlowDirection, typeface, tb.FontSize, tb.Foreground);
            return formattedText.WidthIncludingTrailingWhitespace >= tb.ActualWidth;
        }
        #endregion
    }
}
