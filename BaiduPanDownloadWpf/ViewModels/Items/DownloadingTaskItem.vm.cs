using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Events;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace BaiduPanDownloadWpf.ViewModels.Items
{
    internal class DownloadingTaskItemViewModel : DownloadTaskItemViewModel
    {
        private DataSize _downloadSpeed;
        private DataSize _downloadProgerss;
        private DownloadStateEnum _downloadState;

        private Command _pauseTaskCommand;
        private Command _startTaskCommand;
        private Command _cancelTaskCommand;

        public DownloadingTaskItemViewModel(IUnityContainer container, IDiskFile diskFile)
            : base(container, diskFile)
        {
            PauseTaskCommand = new Command(PauseTaskCommandExecute, PauseTaskCommandCanExecute);
            StartTaskCommand = new Command(StartTaskCommandExecute, StartTaskCommandCanExecute);
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

        public TimeSpan RemainingTime => new TimeSpan(0, 0, (int)Math.Round((1.0 * (FileSize?.ValueBasedOnB ?? 0) - DownloadProgress.ValueBasedOnB) / DownloadSpeed.ValueBasedOnB));
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
            ChangeDownloadStateWithPublishEvent(DownloadStateEnum.Canceled);
            //_tokenSource.Cancel(); // TODO: Mock data to delete.
        }
        private bool StartTaskCommandCanExecute()
        {
            return DownloadState != DownloadStateEnum.Canceled && DownloadState != DownloadStateEnum.Downloading;
        }
        private void StartTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Start Task: {FilePath.FullPath} Command");
            //_tokenSource?.Dispose();
            //_tokenSource = new CancellationTokenSource();
            //MockDownload(_tokenSource.Token);
        }
        private bool PauseTaskCommandCanExecute()
        {
            return DownloadState == DownloadStateEnum.Downloading;
        }
        private void PauseTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Pause Task: {FilePath.FullPath} Command");
            //_tokenSource?.Cancel();
        }
        #endregion

        #region Events
        private void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            var oldProgress = DownloadProgress;
            DownloadProgress = e.CurrentProgress;
            DownloadSpeed = (DownloadProgress - oldProgress);// / (e.Timestamp - _lastTime).TotalSeconds;
            OnPropertyChanged(nameof(RemainingTime));
        }
        private void OnDownloadStateChanged(DownloadStateChangedEventArgs e)
        {
            DownloadSpeed = new DataSize();
            DownloadState = e.NewState;
        }
        private void ChangeDownloadStateWithPublishEvent(DownloadStateEnum state)
        {
            if (DownloadState == state) return;
            var temp = DownloadState;
            DownloadState = state;
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Publish(new DownloadStateChangedEventArgs(FileId, temp, DownloadState));
        }
        #endregion
    }
}
