using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MusicPlayer.Extensions;
using MusicPlayer.Helpers;
using MusicPlayer.Models;

namespace MusicPlayer.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MusicPlayHelper MusicPlayHelper { get; }

        public ConfigInfo ConfigInfo => GlobalInfo.ConfigInfo;

        private ObservableCollection<MusicInfo> _musicInfoList;
        public ObservableCollection<MusicInfo> MusicInfoList
        {
            get { return _musicInfoList; }
            set { Set("MusicInfoList", ref _musicInfoList, value); }
        }

        #region 命令
        public RelayCommand WindowLoadedCommand { get; }
        public RelayCommand WindowClosingCommand { get; }
        public RelayCommand<IList> SetSelectedStatusCommand { get; }
        public RelayCommand AddMusicFileCommand { get; }
        public RelayCommand AddMusicFolderCommand { get; }
        public RelayCommand DeleteSelectMusicCommand { get; }
        public RelayCommand DeleteRepeatMusicCommand { get; }
        public RelayCommand DeleteErrorMusicCommand { get; }
        public RelayCommand ClearMusicListCommand { get; }
        public RelayCommand<int> DeleteMusicByAddTimeCommand { get; }
        public RelayCommand<int> DeleteMusicByPlayTimesCommand { get; }
        public RelayCommand<PlayMode> SetPlayModeCommand { get; }
        public RelayCommand ChangePlayModeCommand { get; }
        public RelayCommand<OrderMode> OrderByCommand { get; }
        public RelayCommand StopCommand { get; }
        public RelayCommand PrevCommand { get; }
        public RelayCommand<MusicInfo> PlayPauseCommand { get; }
        public RelayCommand<bool> NextCommand { get; }
        public RelayCommand MuteCommand { get; }
        public RelayCommand VolumeChangedCommand { get; }
        public RelayCommand VolumeAddCommand { get; }
        public RelayCommand VolumeSubtractCommand { get; }
        public RelayCommand PlayProgressChangedCommand { get; }
        #endregion

        public MainWindowViewModel()
        {
            MusicInfoList = new ObservableCollection<MusicInfo>(XmlHelper.GetMusicInfoList());
            MusicPlayHelper = new MusicPlayHelper(MusicInfoList.FirstOrDefault(t => t.PlayStatus != PlayStatus.Normal));
            MusicPlayHelper.SetIsMuted(ConfigInfo.IsMuted);
            MusicPlayHelper.SetVolume(ConfigInfo.Volume);

            WindowLoadedCommand = new RelayCommand(() =>
            {
                if (ConfigInfo.AutoPlay) PlayPauseCommand.Execute(null);
            });
            WindowClosingCommand = new RelayCommand(() =>
            {
                MusicPlayHelper.Stop();
                XmlHelper.SaveConfigInfo(ConfigInfo);
                XmlHelper.SaveMusicInfoList(MusicInfoList.ToList());
            });
            SetSelectedStatusCommand = new RelayCommand<IList>(list =>
            {
                List<MusicInfo> select = list.Cast<MusicInfo>().ToList();
                MusicInfoList.ForEach(info =>
                {
                    info.IsSelected = select.Any(t => t.FilePath == info.FilePath);
                });
            });
            AddMusicFileCommand = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "选择文件",
                    Filter = "音乐文件(*.mp3;*.wma;*.m4a;*.wav;)|*.mp3;*.wma;*.m4a;*.wav;",
                    FileName = string.Empty,
                    RestoreDirectory = true,
                    Multiselect = true
                };
                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.Cancel)
                {
                    return;
                }

                AddMusicFile(openFileDialog.FileNames);
            });
            AddMusicFolderCommand = new RelayCommand(() =>
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog { Description = "选择一个目录" };
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.Cancel)
                {
                    return;
                }

                string[] filePaths = "*.mp3;*.wma;*.m4a;*.wav".Split(';').SelectMany(filter => Directory.GetFiles(folderBrowserDialog.SelectedPath, filter, SearchOption.AllDirectories)).ToArray();
                AddMusicFile(filePaths);
            });
            DeleteSelectMusicCommand = new RelayCommand(() =>
            {
                DeleteMusic(info => info.IsSelected);
            });
            DeleteRepeatMusicCommand = new RelayCommand(() =>
            {
                var result = MusicInfoList.Distinct(new MusicInfoNoComparer());
                SetMusicInfoList(result);
                if (MusicInfoList.All(info => info != MusicPlayHelper.PlayMusicInfo))
                {
                    StopAndPlayNext();
                }
            });
            DeleteErrorMusicCommand = new RelayCommand(() =>
            {
                DeleteMusic(info => !File.Exists(info.FilePath));
            });
            ClearMusicListCommand = new RelayCommand(() =>
            {
                MusicInfoList.Clear();
                StopAndPlayNext();
            });
            DeleteMusicByAddTimeCommand = new RelayCommand<int>(day =>
            {
                var date = DateTime.Now.AddDays(-day);
                DeleteMusic(info => info.AddTime < date);
            });
            DeleteMusicByPlayTimesCommand = new RelayCommand<int>(playTimes =>
            {
                if (playTimes == 0)
                {
                    DeleteMusic(info => info.PlayTimes == 0 && info.PlayStatus != PlayStatus.Play);
                }
                else
                {
                    DeleteMusic(info => info.PlayTimes < playTimes);
                }
            });
            SetPlayModeCommand = new RelayCommand<PlayMode>(playMode =>
            {
                ConfigInfo.PlayMode = playMode;
            });
            ChangePlayModeCommand = new RelayCommand(() =>
            {
                var playMode = (int)ConfigInfo.PlayMode + 1;
                if (playMode > 3) playMode = 0;
                ConfigInfo.PlayMode = (PlayMode)playMode;
            });
            OrderByCommand = new RelayCommand<OrderMode>(order =>
            {
                IEnumerable<MusicInfo> result;
                if (ConfigInfo.OrderMode == order)
                {
                    if (ConfigInfo.Sort == Sort.Asc)
                    {
                        ConfigInfo.Sort = Sort.Desc;
                        result = MusicInfoList.OrderByDescending(GlobalInfo.OrderModeFuncs[(int)order]);
                    }
                    else
                    {
                        ConfigInfo.Sort = Sort.Asc;
                        result = MusicInfoList.OrderBy(GlobalInfo.OrderModeFuncs[(int)order]);
                    }
                }
                else
                {
                    ConfigInfo.Sort = Sort.Asc;
                    ConfigInfo.OrderMode = order;
                    result = MusicInfoList.OrderBy(GlobalInfo.OrderModeFuncs[(int)order]);
                }

                SetMusicInfoList(result);
            });
            StopCommand = new RelayCommand(() =>
            {
                MusicPlayHelper.Stop();
                ConfigInfo.PlayStatus = PlayStatus.Pause;
            });
            PrevCommand = new RelayCommand(() =>
            {
                if (MusicInfoList.Count == 0)
                {
                    return;
                }
                MusicInfo playMusic;
                switch (ConfigInfo.PlayMode)
                {
                    case PlayMode.RandomPlay:
                        int index = RandomHelper.GetNextIndex(MusicInfoList.Count, MusicPlayHelper.PlayMusicInfo.RowNum - 1);
                        playMusic = MusicInfoList[index];
                        break;
                    default:
                        if (MusicPlayHelper.PlayMusicInfo == null)
                        {
                            playMusic = MusicInfoList.FirstOrDefault();
                        }
                        else
                        {
                            index = MusicPlayHelper.PlayMusicInfo.RowNum - 2;
                            playMusic = index > -1 ? MusicInfoList[index] : MusicInfoList.LastOrDefault();
                        }
                        break;
                }
                MusicPlayHelper.PlayPause(playMusic);
            });
            PlayPauseCommand = new RelayCommand<MusicInfo>(info =>
            {
                var playMusic = info ?? MusicPlayHelper.PlayMusicInfo ?? MusicInfoList.FirstOrDefault();
                MusicPlayHelper.PlayPause(playMusic);
            });
            NextCommand = new RelayCommand<bool>((isPlayEnd) =>
            {
                if (MusicInfoList.Count == 0)
                {
                    return;
                }
                MusicInfo playMusic;
                switch (ConfigInfo.PlayMode)
                {
                    case PlayMode.RandomPlay:
                        int index = RandomHelper.GetNextIndex(MusicInfoList.Count, MusicPlayHelper.PlayMusicInfo.RowNum - 1);
                        playMusic = MusicInfoList[index];
                        break;
                    default:
                        if (MusicPlayHelper.PlayMusicInfo == null)
                        {
                            playMusic = MusicInfoList.FirstOrDefault();
                        }
                        else
                        {
                            if (ConfigInfo.PlayMode == PlayMode.SinglePlay && isPlayEnd)
                            {
                                playMusic = MusicPlayHelper.PlayMusicInfo;
                                MusicPlayHelper.SetPlayMusicInfo(null);
                            }
                            else if (ConfigInfo.PlayMode == PlayMode.SequentialPlay && MusicPlayHelper.PlayMusicInfo.RowNum == MusicInfoList.Count && isPlayEnd)
                            {
                                MusicPlayHelper.Finish();
                                ConfigInfo.PlayStatus = PlayStatus.Pause;
                                return;
                            }
                            else
                            {
                                index = MusicPlayHelper.PlayMusicInfo.RowNum;
                                playMusic = index < MusicInfoList.Count ? MusicInfoList[index] : MusicInfoList.FirstOrDefault();
                            }
                        }
                        break;
                }
                MusicPlayHelper.PlayPause(playMusic);
            });
            MuteCommand = new RelayCommand(() =>
            {
                ConfigInfo.IsMuted = !ConfigInfo.IsMuted;
                MusicPlayHelper.SetIsMuted(ConfigInfo.IsMuted);
            });
            VolumeChangedCommand = new RelayCommand(() =>
            {
                MusicPlayHelper.SetVolume(ConfigInfo.Volume);
            });
            VolumeAddCommand = new RelayCommand(() =>
            {
                var value = ConfigInfo.Volume + 0.05;
                if (value <= 1)
                {
                    ConfigInfo.Volume = value;
                    MusicPlayHelper.SetVolume(ConfigInfo.Volume);
                }
            });
            VolumeSubtractCommand = new RelayCommand(() =>
            {
                var value = ConfigInfo.Volume - 0.05;
                if (value >= 0)
                {
                    ConfigInfo.Volume = value;
                    MusicPlayHelper.SetVolume(ConfigInfo.Volume);
                }
            });
            PlayProgressChangedCommand = new RelayCommand(() =>
            {
                MusicPlayHelper.SetPosition(ConfigInfo.Position);
            });
        }

        private void AddMusicFile(params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (MusicInfoList.All(info => info.FilePath != filePath))
                {
                    MusicInfo musicInfo = MusicInfoHelper.GetMusicInfo(filePath);
                    musicInfo.RowNum = MusicInfoList.Count + 1;
                    musicInfo.AddTime = DateTime.Now;
                    musicInfo.FilePath = filePath;
                    musicInfo.PlayTimes = 0;
                    musicInfo.PlayStatus = PlayStatus.Normal;

                    MusicInfoList.Add(musicInfo);
                }
            }
        }

        private void StopAndPlayNext()
        {
            MusicPlayHelper.Stop();
            if (MusicInfoList.Count == 0)
            {
                ConfigInfo.ResetPlayInfo();
            }
            else
            {
                ConfigInfo.PlayStatus = PlayStatus.Pause;
                NextCommand.Execute(null);
            }
        }

        private void DeleteMusic(Func<MusicInfo, bool> func)
        {
            bool playNext = false;
            int oldIndex = -2;
            for (int i = 0; i < MusicInfoList.Count;)
            {
                MusicInfoList[i].RowNum = i + 1;
                if (func(MusicInfoList[i]))
                {
                    if (MusicInfoList[i] == MusicPlayHelper.PlayMusicInfo)
                    {
                        playNext = true;
                    }
                    MusicInfoList.RemoveAt(i);
                }
                else
                {
                    if (playNext && oldIndex == -2)
                    {
                        oldIndex = i - 1;
                    }
                    i++;
                }
            }
            if (playNext)
            {
                MusicPlayHelper.SetPlayMusicInfo(oldIndex < 0 ? null : MusicInfoList[oldIndex]);
                StopAndPlayNext();
            }
        }

        private void SetMusicInfoList(IEnumerable<MusicInfo> infos)
        {
            MusicInfoList = new ObservableCollection<MusicInfo>(infos.Select((t, i) =>
            {
                t.RowNum = i + 1;
                return t;
            }));
        }
    }
}
