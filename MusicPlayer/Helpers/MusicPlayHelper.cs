using System;
using System.Windows.Media;
using System.Windows.Threading;
using MusicPlayer.Models;

namespace MusicPlayer.Helpers
{
    public class MusicPlayHelper
    {
        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private void Timer_Tick(object obj, EventArgs e)
        {
            MessengerHelper.Send(GlobalInfo.UpdatePlayProgressToken, _mediaPlayer.Position);
        }

        private MusicInfo _playMusicInfo;
        public MusicInfo PlayMusicInfo => _playMusicInfo;

        public MusicPlayHelper(MusicInfo musicInfo)
        {
            _mediaPlayer.MediaEnded += MediaEnded;
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;

            _playMusicInfo = musicInfo;
            if (_playMusicInfo != null)
            {
                _mediaPlayer.Open(new Uri(_playMusicInfo.FilePath, UriKind.Absolute));
            }
        }

        private void MediaEnded(object sender, EventArgs e)
        {
            PlayMusicInfo.PlayTimes++;
            MessengerHelper.Send(GlobalInfo.NextMusicToken, "");
        }

        private void SetMusicInfoPlayStatus(PlayStatus playStatus)
        {
            if (PlayMusicInfo != null) PlayMusicInfo.PlayStatus = playStatus;
        }

        private void Open(MusicInfo musicInfo)
        {
            if (musicInfo != PlayMusicInfo)
            {
                SetMusicInfoPlayStatus(PlayStatus.Normal);
                musicInfo.PlayStatus = PlayStatus.Play;

                _playMusicInfo = musicInfo;
                _mediaPlayer.Open(new Uri(musicInfo.FilePath, UriKind.Absolute));
            }
            else
            {
                musicInfo.PlayStatus = musicInfo.PlayStatus == PlayStatus.Play ? PlayStatus.Pause : PlayStatus.Play;
            }
        }

        public void PlayPause(MusicInfo musicInfo)
        {
            if (musicInfo != null)
            {
                Open(musicInfo);
                musicInfo.Position = _mediaPlayer.Position;
                if (musicInfo.PlayStatus == PlayStatus.Play)
                {
                    _timer.Start();
                    _mediaPlayer.Play();
                }
                else
                {
                    _mediaPlayer.Pause();
                    _timer.Stop();
                }
            }
            MessengerHelper.Send(GlobalInfo.UpdateInfoToken, musicInfo);
        }

        public void Stop()
        {
            SetMusicInfoPlayStatus(PlayStatus.Pause);
            _mediaPlayer.Stop();
            _playMusicInfo = null;
        }

        public void SetIsMuted(bool isMuted)
        {
            _mediaPlayer.IsMuted = isMuted;
        }

        public void SetVolume(double volume)
        {
            _mediaPlayer.Volume = volume;
        }

        public void SetPosition(TimeSpan position)
        {
            _mediaPlayer.Position = position;
        }
    }
}
