using System;
using System.Windows.Controls;
using System.Windows.Input;
using MusicPlayer.Helpers;
using MusicPlayer.Models;

namespace MusicPlayer.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MessengerHelper.Register<string>(this, GlobalInfo.NextMusicToken, message =>
            {
                ViewMode.NextCommand.Execute(null);
            });
            MessengerHelper.Register<MusicInfo>(this, GlobalInfo.UpdateInfoToken, info =>
            {
                if (info == null)
                {
                    ViewMode.ConfigInfo.PlayStatus = PlayStatus.Pause;
                }
                else
                {
                    ViewMode.ConfigInfo.PlayStatus = info.PlayStatus;
                    ViewMode.ConfigInfo.MusicName = info.MusicName;
                    ViewMode.ConfigInfo.Singer = info.Singer;
                    ViewMode.ConfigInfo.TimeLength = info.TimeLength;
                    SetPlayProgress(info.Position);
                }
            });
            MessengerHelper.Register<TimeSpan>(this, GlobalInfo.UpdatePlayProgressToken, SetPlayProgress);
        }

        private void SetPlayProgress(TimeSpan timeSpan)
        {
            if (!MusicSlider.IsMouseDown)
                ViewMode.ConfigInfo.Position = timeSpan;
            ViewMode.ConfigInfo.MusicTime = string.Format("{0:00}:{1:00}|{2}", timeSpan.Minutes, timeSpan.Seconds, ViewMode.ConfigInfo.TimeLength.Substring(3));
        }

        private void MusicList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.DirectlyOver is ScrollViewer)
            {
                MusicList.SelectedValue = null;
            }
        }
    }
}
