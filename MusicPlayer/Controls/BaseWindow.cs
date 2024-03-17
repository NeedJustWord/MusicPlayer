using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.Command;
using MusicPlayer.ViewModel;
using MusicPlayer.Windows;

namespace MusicPlayer.Controls
{
    public class BaseWindow : Window
    {
        private MainWindowViewModel _viewModel;
        public MainWindowViewModel ViewMode => _viewModel ?? (_viewModel = DataContext as MainWindowViewModel);

        private HwndSource _hs;
        private bool _isHide;
        private bool _isInside;
        private bool _isJustMove;
        private const double BorderWidth = 2;
        private const double DoubleBorderWidth = 4;

        public BaseWindow()
        {
            SourceInitialized += (sender, e) =>
            {
                _hs = PresentationSource.FromVisual(this) as HwndSource;
                _hs?.AddHook(WndProc);
            };
            SetOpacityCommand = new RelayCommand<double>(opacity =>
            {
                Opacity = opacity;
            });
            AboutCommand = new RelayCommand(() =>
            {
                AboutWindow about = new AboutWindow();
                about.ShowDialog();
            });
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return new IntPtr(0);
        }

        #region 命令
        public RelayCommand<double> SetOpacityCommand { get; }

        public RelayCommand AboutCommand { get; }
        #endregion

        #region 四个边加上四个角
        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        #endregion

        #region 用于改变窗体大小
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hs.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        #endregion

        #region 用于无边框窗体的移动。在鼠标左键按下时调用，如MouseLeftButtonDown事件中
        #region 法一：重写的DragMove，以便解决利用系统自带的DragMove出现Exception的情况
        // ReSharper disable once UnusedMember.Local
        private new void DragMove()
        {
            if (WindowState == WindowState.Normal)
            {
                SendMessage(_hs.Handle, 0x112, (IntPtr)0xf012, IntPtr.Zero);
                SendMessage(_hs.Handle, 0x0202, IntPtr.Zero, IntPtr.Zero);
            }
        }
        #endregion

        #region 法二：利用系统api调用实现
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private void MoveWindow()
        {
            ReleaseCapture();
            SendMessage(_hs.Handle, 0x0112, 0xf012, 0);
        }
        #endregion
        #endregion

        #region 为元素注册事件
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (PullOverHide)
            {
                MouseEnter += BaseWindow_MouseEnter;
                MouseLeave += BaseWindow_MouseLeave;
            }
            PreviewMouseMove += ResetCursor;

            Border baseWindowBorder = GetTemplateChild("BaseWindowBorder") as Border;
            if (baseWindowBorder != null)
            {
                baseWindowBorder.MouseMove += DisplayResizeCursor;
                baseWindowBorder.PreviewMouseDown += Resize;
            }

            Button btnClose = GetTemplateChild("BtnClose") as Button;
            if (btnClose != null)
            {
                if (HaveCloseButton)
                {
                    btnClose.Click += (sender, e) => Close();
                }
                else
                {
                    btnClose.Visibility = Visibility.Collapsed;
                }
            }

            Button btnMin = GetTemplateChild("BtnMin") as Button;
            if (btnMin != null)
            {
                if (HaveMinButton)
                {
                    btnMin.Click += (sender, e) => WindowState = WindowState.Minimized;
                }
                else
                {
                    btnMin.Visibility = Visibility.Collapsed;
                }
            }

            Button btnMiniStyle = GetTemplateChild("BtnMiniStyle") as Button;
            if (btnMiniStyle != null)
            {
                if (HaveMiniStyleButton)
                {
                    btnMiniStyle.Click += (sender, e) =>
                    {
                        RoutedEventArgs args = new RoutedEventArgs(MiniStyleClickEvent);
                        RaiseEvent(args);
                    };
                }
                else
                {
                    btnMiniStyle.Visibility = Visibility.Collapsed;
                }
            }

            Button btnMenu = GetTemplateChild("BtnMenu") as Button;
            if (btnMenu != null)
            {
                if (HaveMenuButton)
                {
                    btnMenu.Click += (sender, e) =>
                    {
                        RoutedEventArgs args = new RoutedEventArgs(MenuClickEvent);
                        RaiseEvent(args);
                    };
                }
                else
                {
                    btnMenu.Visibility = Visibility.Collapsed;
                }
            }

            Border titleBar = GetTemplateChild("TitleBar") as Border;
            if (titleBar != null)
            {
                if (PullOverHide)
                {
                    titleBar.MouseLeftButtonDown += (sender, e) =>
                    {
                        double top = ViewMode.ConfigInfo.Top;
                        double left = ViewMode.ConfigInfo.Left;
                        double width = ViewMode.ConfigInfo.Width;
                        double height = ViewMode.ConfigInfo.Height;
                        MoveWindow();

                        Point point = PointToScreen(Mouse.GetPosition(this));
                        _isInside = (point.X >= left && point.X <= left + width && point.Y >= top && point.Y <= top + height);
                        _isJustMove = true;
                    };
                }
                else
                {
                    titleBar.MouseLeftButtonDown += (sender, e) =>
                    {
                        MoveWindow();
                    };
                }
            }
        }
        #endregion

        #region 窗体靠边隐藏
        private void HideWindow()
        {
            if (ViewMode.ConfigInfo.Top < 3)
            {
                _isHide = true;
                DoubleAnimation topAnimation = new DoubleAnimation
                {
                    From = ViewMode.ConfigInfo.Top,
                    To = 3 - ViewMode.ConfigInfo.Height,
                    Duration = TimeSpan.FromMilliseconds(100)
                };
                BeginAnimation(TopProperty, topAnimation);
            }
        }

        private void BaseWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isHide)
            {
                _isHide = false;
                DoubleAnimation topAnimation = new DoubleAnimation
                {
                    From = ViewMode.ConfigInfo.Top,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(100)
                };
                BeginAnimation(TopProperty, topAnimation);
            }
        }

        private void BaseWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isJustMove && !_isInside)
            {
                _isJustMove = false;
                _isInside = false;
                return;
            }
            HideWindow();
        }
        #endregion

        #region 显示拖拉鼠标形状
        private void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                return;
            }

            Point pos = Mouse.GetPosition(this);
            double x = pos.X;
            double y = pos.Y;
            double w = ActualWidth;  //注意这个地方使用ActualWidth,才能够实时显示宽度变化
            double h = ActualHeight;

            if (x <= DoubleBorderWidth & y <= DoubleBorderWidth) // left top
            {
                Cursor = Cursors.SizeNWSE;
            }
            if (x >= w - DoubleBorderWidth & y <= DoubleBorderWidth) //right top
            {
                Cursor = Cursors.SizeNESW;
            }

            if (x >= w - DoubleBorderWidth & y >= h - DoubleBorderWidth) //bottom right
            {
                Cursor = Cursors.SizeNWSE;
            }

            if (x <= DoubleBorderWidth & y >= h - DoubleBorderWidth)  // bottom left
            {
                Cursor = Cursors.SizeNESW;
            }

            if ((x >= BorderWidth & x <= w - BorderWidth) & y <= BorderWidth) //top
            {
                Cursor = Cursors.SizeNS;
            }

            if (x >= w - BorderWidth & (y >= BorderWidth & y <= h - BorderWidth)) //right
            {
                Cursor = Cursors.SizeWE;
            }

            if ((x >= BorderWidth & x <= w - BorderWidth) & y > h - BorderWidth) //bottom
            {
                Cursor = Cursors.SizeNS;
            }

            if (x <= BorderWidth & (y <= h - BorderWidth & y >= BorderWidth)) //left
            {
                Cursor = Cursors.SizeWE;
            }
        }
        #endregion

        #region  还原鼠标形状
        private void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Cursor = Cursors.Arrow;
            }
        }
        #endregion

        #region 判断区域，改变窗体大小
        private void Resize(object sender, MouseButtonEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);
            double x = pos.X;
            double y = pos.Y;
            double w = ActualWidth;
            double h = ActualHeight;

            if (x <= DoubleBorderWidth & y <= DoubleBorderWidth) // left top
            {
                Cursor = Cursors.SizeNWSE;
                ResizeWindow(ResizeDirection.TopLeft);
            }
            if (x >= w - DoubleBorderWidth & y <= DoubleBorderWidth) //right top
            {
                Cursor = Cursors.SizeNESW;
                ResizeWindow(ResizeDirection.TopRight);
            }

            if (x >= w - DoubleBorderWidth & y >= h - DoubleBorderWidth) //bottom right
            {
                Cursor = Cursors.SizeNWSE;
                ResizeWindow(ResizeDirection.BottomRight);
            }

            if (x <= DoubleBorderWidth & y >= h - DoubleBorderWidth)  // bottom left
            {
                Cursor = Cursors.SizeNESW;
                ResizeWindow(ResizeDirection.BottomLeft);
            }

            if ((x >= BorderWidth & x <= w - BorderWidth) & y <= BorderWidth) //top
            {
                Cursor = Cursors.SizeNS;
                ResizeWindow(ResizeDirection.Top);
            }

            if (x >= w - BorderWidth & (y >= BorderWidth & y <= h - BorderWidth)) //right
            {
                Cursor = Cursors.SizeWE;
                ResizeWindow(ResizeDirection.Right);
            }

            if ((x >= BorderWidth & x <= w - BorderWidth) & y > h - BorderWidth) //bottom
            {
                Cursor = Cursors.SizeNS;
                ResizeWindow(ResizeDirection.Bottom);
            }

            if (x <= BorderWidth & (y <= h - BorderWidth & y >= BorderWidth)) //left
            {
                Cursor = Cursors.SizeWE;
                ResizeWindow(ResizeDirection.Left);
            }
        }
        #endregion

        #region 自定义事件
        public static readonly RoutedEvent MiniStyleClickEvent = EventManager.RegisterRoutedEvent("MiniStyleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BaseWindow));

        public event RoutedEventHandler MiniStyleClick
        {
            add { AddHandler(MiniStyleClickEvent, value); }
            remove { RemoveHandler(MiniStyleClickEvent, value); }
        }

        public static readonly RoutedEvent MenuClickEvent = EventManager.RegisterRoutedEvent("MenuClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BaseWindow));

        public event RoutedEventHandler MenuClick
        {
            add { AddHandler(MenuClickEvent, value); }
            remove { RemoveHandler(MenuClickEvent, value); }
        }
        #endregion

        #region 自定义属性
        public static DependencyProperty PullOverHideProperty = DependencyProperty.Register("PullOverHide", typeof(bool), typeof(BaseWindow));
        public bool PullOverHide
        {
            get { return (bool)GetValue(PullOverHideProperty); }
            set { SetValue(PullOverHideProperty, value); }
        }

        public static DependencyProperty HaveCloseButtonProperty = DependencyProperty.Register("HaveCloseButton", typeof(bool), typeof(BaseWindow));
        public bool HaveCloseButton
        {
            get { return (bool)GetValue(HaveCloseButtonProperty); }
            set { SetValue(HaveCloseButtonProperty, value); }
        }

        public static DependencyProperty HaveMinButtonProperty = DependencyProperty.Register("HaveMinButton", typeof(bool), typeof(BaseWindow));
        public bool HaveMinButton
        {
            get { return (bool)GetValue(HaveMinButtonProperty); }
            set { SetValue(HaveMinButtonProperty, value); }
        }

        public static DependencyProperty HaveMiniStyleButtonProperty = DependencyProperty.Register("HaveMiniStyleButton", typeof(bool), typeof(BaseWindow));
        public bool HaveMiniStyleButton
        {
            get { return (bool)GetValue(HaveMiniStyleButtonProperty); }
            set { SetValue(HaveMiniStyleButtonProperty, value); }
        }

        public static DependencyProperty HaveMenuButtonProperty = DependencyProperty.Register("HaveMenuButton", typeof(bool), typeof(BaseWindow));
        public bool HaveMenuButton
        {
            get { return (bool)GetValue(HaveMenuButtonProperty); }
            set { SetValue(HaveMenuButtonProperty, value); }
        }

        public static DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(BaseWindow));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
        #endregion
    }
}
