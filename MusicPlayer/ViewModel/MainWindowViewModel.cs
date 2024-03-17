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
        private MusicPlayHelper _musicPlayHelper;
        public MusicPlayHelper MusicPlayHelper => _musicPlayHelper;

        public ConfigInfo ConfigInfo { get; set; }

        private ObservableCollection<MusicInfo> _musicInfoList;
        public ObservableCollection<MusicInfo> MusicInfoList
        {
            get { return _musicInfoList; }
            set { Set("MusicInfoList", ref _musicInfoList, value); }
        }

        public MainWindowViewModel()
        {
            ConfigInfo = XmlHelper.GetConfigInfo();
            MusicInfoList = new ObservableCollection<MusicInfo>(XmlHelper.GetMusicInfoList());
        }

        #region 保存配置文件和音乐列表
        private void SaveConfigInfo()
        {
            XmlHelper.SaveConfigInfo(ConfigInfo);
        }

        private void SaveMusicInfoList()
        {
            XmlHelper.SaveMusicInfoList(MusicInfoList.ToList());
        }

        private void SaveInfo()
        {
            SaveConfigInfo();
            SaveMusicInfoList();
        }
        #endregion

        private RelayCommand _windowLoadedCommand;
        public RelayCommand WindowLoadedCommand
        {
            get
            {
                return _windowLoadedCommand ?? (_windowLoadedCommand = new RelayCommand(() =>
                {
                    var playMusic = MusicInfoList.Where(t => t.PlayStatus != PlayStatus.Normal).Select(t => t).FirstOrDefault();
                    _musicPlayHelper = new MusicPlayHelper(playMusic);
                    MusicPlayHelper.SetIsMuted(ConfigInfo.IsMuted);
                    MusicPlayHelper.SetVolume(ConfigInfo.Volume);
                }));
            }
        }

        private RelayCommand _windowClosingCommand;
        public RelayCommand WindowClosingCommand
        {
            get
            {
                return _windowClosingCommand ?? (_windowClosingCommand = new RelayCommand(() =>
                {
                    MusicPlayHelper.Stop();
                    SaveInfo();
                }));
            }
        }

        private RelayCommand<IList> _setSelectedStatusCommand;
        public RelayCommand<IList> SetSelectedStatusCommand
        {
            get
            {
                return _setSelectedStatusCommand ?? (_setSelectedStatusCommand = new RelayCommand<IList>(list =>
                {
                    List<MusicInfo> select = list.Cast<MusicInfo>().ToList();
                    MusicInfoList.ForEach(info =>
                    {
                        info.IsSelected = select.Any(t => t.FilePath == info.FilePath);
                    });
                }));
            }
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

        private RelayCommand _addMusicFileCommand;
        public RelayCommand AddMusicFileCommand
        {
            get
            {
                return _addMusicFileCommand ?? (_addMusicFileCommand = new RelayCommand(() =>
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
                }));
            }
        }

        private RelayCommand _addMusicFolderCommand;
        public RelayCommand AddMusicFolderCommand
        {
            get
            {
                return _addMusicFolderCommand ?? (_addMusicFolderCommand = new RelayCommand(() =>
                {
                    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog { Description = "选择一个目录" };
                    DialogResult result = folderBrowserDialog.ShowDialog();

                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }

                    string[] filePaths = "*.mp3;*.wma;*.m4a;*.wav".Split(';').SelectMany(filter => Directory.GetFiles(folderBrowserDialog.SelectedPath, filter, SearchOption.AllDirectories)).ToArray();
                    AddMusicFile(filePaths);
                }));
            }
        }

        private void StopAndPlayNext()
        {
            MusicPlayHelper.Stop();
            ConfigInfo.PlayStatus = PlayStatus.Pause;
            NextCommand.Execute(null);
        }

        private void DeleteMusic(Func<MusicInfo, bool> func)
        {
            bool playNext = false;
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
                    i++;
                }
            }
            if (playNext)
            {
                StopAndPlayNext();
            }
        }

        private RelayCommand _deleteSelectMusicCommand;
        public RelayCommand DeleteSelectMusicCommand
        {
            get
            {
                return _deleteSelectMusicCommand ?? (_deleteSelectMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => info.IsSelected);
                }));
            }
        }

        private RelayCommand _deleteRepeatMusicCommand;
        public RelayCommand DeleteRepeatMusicCommand
        {
            get
            {
                return _deleteRepeatMusicCommand ?? (_deleteRepeatMusicCommand = new RelayCommand(() =>
                {
                    var result = MusicInfoList.Distinct(new MusicInfoNoComparer());
                    MusicInfoList = new ObservableCollection<MusicInfo>(result);
                    if (MusicInfoList.All(info => info != MusicPlayHelper.PlayMusicInfo))
                    {
                        StopAndPlayNext();
                    }
                }));
            }
        }

        private RelayCommand _deleteErrorMusicCommand;
        public RelayCommand DeleteErrorMusicCommand
        {
            get
            {
                return _deleteErrorMusicCommand ?? (_deleteErrorMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => !File.Exists(info.FilePath));
                }));
            }
        }

        private RelayCommand _clearMusicListCommand;
        public RelayCommand ClearMusicListCommand
        {
            get
            {
                return _clearMusicListCommand ?? (_clearMusicListCommand = new RelayCommand(() =>
                {
                    MusicInfoList.Clear();
                    StopAndPlayNext();
                }));
            }
        }

        private RelayCommand _deleteLastDayAddMusicCommand;
        public RelayCommand DeleteLastDayAddMusicCommand
        {
            get
            {
                return _deleteLastDayAddMusicCommand ?? (_deleteLastDayAddMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => info.AddTime < DateTime.Now.AddDays(-1));
                }));
            }
        }

        private RelayCommand _deleteLastWeekAddMusicCommand;
        public RelayCommand DeleteLastWeekAddMusicCommand
        {
            get
            {
                return _deleteLastWeekAddMusicCommand ?? (_deleteLastWeekAddMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => info.AddTime < DateTime.Now.AddDays(-7));
                }));
            }
        }

        private RelayCommand _deleteLastMonthAddMusicCommand;
        public RelayCommand DeleteLastMonthAddMusicCommand
        {
            get
            {
                return _deleteLastMonthAddMusicCommand ?? (_deleteLastMonthAddMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => info.AddTime < DateTime.Now.AddDays(-30));
                }));
            }
        }

        private RelayCommand _deleteNeverPlayMusicCommand;
        public RelayCommand DeleteNeverPlayMusicCommand
        {
            get
            {
                return _deleteNeverPlayMusicCommand ?? (_deleteNeverPlayMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => info.PlayTimes == 0 && info.PlayStatus != PlayStatus.Play);
                }));
            }
        }

        private RelayCommand _deletePlayThreeTimesMusicCommand;
        public RelayCommand DeletePlayThreeTimesMusicCommand
        {
            get
            {
                return _deletePlayThreeTimesMusicCommand ?? (_deletePlayThreeTimesMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => info.PlayTimes < 3);
                }));
            }
        }

        private RelayCommand _deletePlaySixTimesMusicCommand;
        public RelayCommand DeletePlaySixTimesMusicCommand
        {
            get
            {
                return _deletePlaySixTimesMusicCommand ?? (_deletePlaySixTimesMusicCommand = new RelayCommand(() =>
                {
                    DeleteMusic(info => info.PlayTimes < 6);
                }));
            }
        }

        private RelayCommand<PlayMode> _setPlayModeCommand;
        public RelayCommand<PlayMode> SetPlayModeCommand
        {
            get
            {
                return _setPlayModeCommand ?? (_setPlayModeCommand = new RelayCommand<PlayMode>(playMode =>
                {
                    ConfigInfo.PlayMode = playMode;
                }));
            }
        }

        private RelayCommand<OrderMode> _orderByCommand;
        public RelayCommand<OrderMode> OrderByCommand
        {
            get
            {
                return _orderByCommand ?? (_orderByCommand = new RelayCommand<OrderMode>(func =>
                {
                    IEnumerable<MusicInfo> result;
                    if (ConfigInfo.OrderMode == func)
                    {
                        if (ConfigInfo.Sort == Sort.Asc)
                        {
                            ConfigInfo.Sort = Sort.Desc;
                            result = MusicInfoList.OrderByDescending(GlobalInfo.OrderModeFuncs[(int)func]);
                        }
                        else
                        {
                            ConfigInfo.Sort = Sort.Asc;
                            result = MusicInfoList.OrderBy(GlobalInfo.OrderModeFuncs[(int)func]);
                        }
                    }
                    else
                    {
                        ConfigInfo.Sort = Sort.Desc;
                        ConfigInfo.OrderMode = func;
                        result = MusicInfoList.OrderByDescending(GlobalInfo.OrderModeFuncs[(int)func]);
                    }

                    MusicInfoList = new ObservableCollection<MusicInfo>(result);
                    for (int i = 0; i < MusicInfoList.Count;)
                    {
                        MusicInfoList[i].RowNum = ++i;
                    }
                }));
            }
        }

        private RelayCommand _stopCommand;
        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand ?? (_stopCommand = new RelayCommand(() =>
                {
                    MusicPlayHelper.Stop();
                    ConfigInfo.PlayStatus = PlayStatus.Pause;
                }));
            }
        }

        private RelayCommand _prevCommand;
        public RelayCommand PrevCommand
        {
            get
            {
                return _prevCommand ?? (_prevCommand = new RelayCommand(() =>
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
                }));
            }
        }

        private RelayCommand<MusicInfo> _playPauseCommand;
        public RelayCommand<MusicInfo> PlayPauseCommand
        {
            get
            {
                return _playPauseCommand ?? (_playPauseCommand = new RelayCommand<MusicInfo>(info =>
                {
                    var playMusic = info ?? (MusicPlayHelper.PlayMusicInfo ?? MusicInfoList.FirstOrDefault());
                    MusicPlayHelper.PlayPause(playMusic);
                }));
            }
        }

        private RelayCommand<bool> _nextCommand;
        public RelayCommand<bool> NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new RelayCommand<bool>((isPlayEnd) =>
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
                }));
            }
        }

        private RelayCommand _muteCommand;
        public RelayCommand MuteCommand
        {
            get
            {
                return _muteCommand ?? (_muteCommand = new RelayCommand(() =>
                {
                    ConfigInfo.IsMuted = !ConfigInfo.IsMuted;
                    MusicPlayHelper.SetIsMuted(ConfigInfo.IsMuted);
                }));
            }
        }

        private RelayCommand _volumeChangedCommand;
        public RelayCommand VolumeChangedCommand
        {
            get
            {
                return _volumeChangedCommand ?? (_volumeChangedCommand = new RelayCommand(() =>
                {
                    MusicPlayHelper.SetVolume(ConfigInfo.Volume);
                }));
            }
        }

        private RelayCommand _playProgressChangedCommand;
        public RelayCommand PlayProgressChangedCommand
        {
            get
            {
                return _playProgressChangedCommand ?? (_playProgressChangedCommand = new RelayCommand(() =>
                {
                    MusicPlayHelper.SetPosition(ConfigInfo.Position);
                }));
            }
        }
    }
}
