using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Events;
using BaiduPanDownloadWpf.ViewModels.Items;
using Microsoft.Practices.Unity;
using Prism.Events;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal class DownloadingPageViewModel : ViewModelBase
    {
        private ObservableCollection<DownloadingTaskItemViewModel> _downloadTaskList = new ObservableCollection<DownloadingTaskItemViewModel>();


        public DownloadingPageViewModel(IUnityContainer container, ILocalDiskUserRepository localDiskUserRepository)
            : base(container)
        {
            var localDiskUser = localDiskUserRepository.FirstOrDefault();
            if (localDiskUser?.CurrentNetDiskUser != null)
            {
                foreach (var item in localDiskUser.CurrentNetDiskUser.GetUncompletedFiles())
                {
                    DownloadTaskList.Add(Container.Resolve<DownloadingTaskItemViewModel>(new DependencyOverride<IDiskFile>(item)));
                }
            }
            PauseAllCommand = new Command(PauseAllCommandExecute, () => (bool) DownloadTaskList?.Any());
            StartAllCommand = new Command(StartAllCommandExecute, () => DownloadTaskList.Any());
            CancelAllCommand = new Command(CancelAllCommandExecute, () => (bool) DownloadTaskList?.Any());

            EventAggregator.GetEvent<DownloadProgressChangedEvent>().Subscribe(OnDownloadPregressChanged, ThreadOption.UIThread);
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Subscribe(OnDownloadStateChanged, ThreadOption.UIThread);
        }


        public ObservableCollection<DownloadingTaskItemViewModel> DownloadTaskList
        {
            get { return _downloadTaskList; }
            set { SetProperty(ref _downloadTaskList, value); }
        }
        public DataSize TotalDownloadProgress => DownloadTaskList.Aggregate(new DataSize(), (current, item) => current + item.DownloadProgress);
        public DataSize TotalDownloadSpeed => DownloadTaskList.Aggregate(new DataSize(), (current, item) => current + item.DownloadSpeed);
        public DataSize TotalDownloadQuantity => !DownloadTaskList.Any() ? new DataSize(1) : DownloadTaskList.Aggregate(new DataSize(), (current, item) => current + item.FileSize ?? default(DataSize));
        public bool IsStartAll => DownloadTaskList.All(temp => temp.StartTaskCommand.CanExecute());

        #region Commands and their logic
        private Command _pauseAllCommand;
        private Command _startAllCommand;
        private Command _cancelAllCommand;

        public Command PauseAllCommand
        {
            get { return _pauseAllCommand; }
            set { SetProperty(ref _pauseAllCommand, value); }
        }
        public Command StartAllCommand
        {
            get { return _startAllCommand; }
            set { SetProperty(ref _startAllCommand, value); }
        }
        public Command CancelAllCommand
        {
            get { return _cancelAllCommand; }
            set { SetProperty(ref _cancelAllCommand, value); }
        }

        private void CancelAllCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Cancel All Command");
            foreach (var item in DownloadTaskList)
            {
                if (item.CancelTaskCommand.CanExecute())
                {
                    item.CancelTaskCommand.Execute();
                }
            }
        }
        private void StartAllCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Start All Command, IsStartAll = {IsStartAll}");
            foreach (var item in DownloadTaskList)
            {
                if (item.StartTaskCommand.CanExecute())
                {
                    item.StartTaskCommand.Execute();
                }
            }
            OnPropertyChanged(nameof(IsStartAll));
        }
        private void PauseAllCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Pause All Command, IsStartAll = {IsStartAll}");
            foreach (var item in DownloadTaskList)
            {
                if (item.PauseTaskCommand.CanExecute())
                {
                    item.PauseTaskCommand.Execute();
                }
            }
            OnPropertyChanged(nameof(IsStartAll));
        }
        #endregion

        private void OnDownloadPregressChanged(DownloadProgressChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalDownloadProgress));
            OnPropertyChanged(nameof(TotalDownloadSpeed));
        }
        private void OnDownloadStateChanged(DownloadStateChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsStartAll));
            if (e.NewState == DownloadStateEnum.Completed || e.NewState == DownloadStateEnum.Canceled)
                OnDownloadCompletedOrCanceled(e);
            Debug.WriteLine($"{DateTime.Now}: FileId={e.FileId}, OldState={e.OldState}, NewState={e.NewState}, IsStartAll={IsStartAll}");
        }
        private void OnDownloadCompletedOrCanceled(DownloadStateChangedEventArgs e)
        {
            DownloadTaskList.Remove(DownloadTaskList.FirstOrDefault(item => item.FileId == e.FileId));
            OnPropertyChanged(nameof(TotalDownloadQuantity));
            OnDownloadPregressChanged(null);
        }
    }
}
