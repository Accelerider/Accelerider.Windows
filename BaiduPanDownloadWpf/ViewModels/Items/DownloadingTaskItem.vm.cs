using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Events;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace BaiduPanDownloadWpf.ViewModels.Items
{
    internal class DownloadingTaskItemViewModel : DownloadTaskItemViewModel
    {
        // TODO: Temporary solution.
        private IMountUser _localDiskUser;

        private DataSize _downloadSpeed;
        private DataSize _downloadProgerss;
        private DownloadStateEnum _downloadState = DownloadStateEnum.Waiting;

        private Command _pauseTaskCommand;
        private Command _startTaskCommand;
        private Command _cancelTaskCommand;

        public DownloadingTaskItemViewModel(IUnityContainer container, IMountUser localDiskUser, IDiskFile diskFile)
            : base(container, diskFile)
        {
            // TODO: Temporary solution.
            _localDiskUser = localDiskUser;

            PauseTaskCommand = new Command(PauseTaskCommandExecute, () => DownloadState == DownloadStateEnum.Downloading || DownloadState == DownloadStateEnum.Waiting);
            StartTaskCommand = new Command(StartTaskCommandExecute, () => DownloadState == DownloadStateEnum.Paused);
            CancelTaskCommand = new Command(CancelTaskCommandExecute);

            EventAggregator.GetEvent<DownloadProgressChangedEvent>().Subscribe(OnDownloadProgressChanged,
                ThreadOption.UIThread,
                keepSubscriberReferenceAlive: false,
                filter: e => e.FileId == FileId);
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Subscribe(OnDownloadStateChanged,
                ThreadOption.UIThread,
                keepSubscriberReferenceAlive: false,
                filter: e => e.FileId == FileId);
        }

        public TimeSpan? RemainingTime
        {
            get
            {
                if (DownloadSpeed.BaseBValue == 0)
                    return null;
                return new TimeSpan(0, 0, (int)Math.Round((1.0 * (FileSize?.BaseBValue ?? 0) - DownloadProgress.BaseBValue) / DownloadSpeed.BaseBValue));
            }
        }

        public DataSize DownloadSpeed
        {
            get { return _downloadSpeed; }
            private set { SetProperty(ref _downloadSpeed, value); }
        }
        public DataSize DownloadProgress
        {
            get { return _downloadProgerss; }
            private set { SetProperty(ref _downloadProgerss, value); }
        }
        public DownloadStateEnum DownloadState
        {
            get { return _downloadState; }
            private set { SetProperty(ref _downloadState, value); }
        }

        #region Commands and logic
        public Command PauseTaskCommand
        {
            get { return _pauseTaskCommand; }
            set { SetProperty(ref _pauseTaskCommand, value); }
        }
        public Command StartTaskCommand
        {
            get { return _startTaskCommand; }
            set { SetProperty(ref _startTaskCommand, value); }
        }
        public Command CancelTaskCommand
        {
            get { return _cancelTaskCommand; }
            set { SetProperty(ref _cancelTaskCommand, value); }
        }

        private void CancelTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Cancel Task: {FilePath.FullPath} Command");
            // TODO: TryCancel() the task.
            _localDiskUser.CancelDownloadTask(FileId);
        }
        private void StartTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Start Task: {FilePath.FullPath} Command");
            // TODO: TryStart() the task.
            _localDiskUser.RestartDownloadTask(FileId);
        }
        private void PauseTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Pause Task: {FilePath.FullPath} Command");
            // TODO: TryPause() the task.
            _localDiskUser.PasueDownloadTask(FileId);
        }
        #endregion

        #region Events
        private void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            // TODO: Temporary solution.
            if(DownloadState != DownloadStateEnum.Downloading)
            {
                var temp = DownloadState;
                DownloadState = DownloadStateEnum.Downloading;
                EventAggregator.GetEvent<DownloadStateChangedEvent>().Publish(new DownloadStateChangedEventArgs(FileId, temp, DownloadState));
            }
            DownloadProgress = e.CurrentProgress;
            DownloadSpeed = e.CurrentSpeed;
            OnPropertyChanged(nameof(RemainingTime));
        }
        private void OnDownloadStateChanged(DownloadStateChangedEventArgs e)
        {
            DownloadSpeed = new DataSize();
            DownloadState = e.NewState;
        }
        #endregion
    }
}
