using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Infrastructure.Events;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using BaiduPanDownloadWpf.ViewModels.Items;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal class DownloadedPageViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;
        private INetDiskUser _netDiskUser;
        private ObservableCollection<DownloadedTaskItemViewModel> _downloadTaskList = new ObservableCollection<DownloadedTaskItemViewModel>();

        public DownloadedPageViewModel(IUnityContainer container, IMountUserRepository mountUserRepository)
            : base(container)
        {
            _mountUserRepository = mountUserRepository;

            ClearAllRecordCommand = new Command(ClearAllRecordCommandExecute, () => DownloadTaskList?.Any() ?? false);
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Subscribe(
                OnDownloadCompleted,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: false,
                filter: e => e.NewState == DownloadStateEnum.Completed);
        }

        protected override void OnLoaded()
        {
            if (!SetProperty(ref _netDiskUser, _mountUserRepository?.FirstOrDefault()?.GetCurrentNetDiskUser())) return;
            if (_netDiskUser == null)
            {
                DownloadTaskList.Clear();
                return;
            }
            foreach (var item in _netDiskUser.GetCompletedFiles())
            {
                if (DownloadTaskList.Any(element => element.FileId == item.FileId)) continue;
                DownloadTaskList.Add(Container.Resolve<DownloadedTaskItemViewModel>(new DependencyOverride<ILocalDiskFile>(item)));
            }
        }

        public ObservableCollection<DownloadedTaskItemViewModel> DownloadTaskList
        {
            get { return _downloadTaskList; }
            set { SetProperty(ref _downloadTaskList, value); }
        }


        private ICommand _clearAllRecordCommand;

        public ICommand ClearAllRecordCommand
        {
            get { return _clearAllRecordCommand; }
            set { SetProperty(ref _clearAllRecordCommand, value); }
        }

        private void ClearAllRecordCommandExecute()
        {
            foreach (var item in DownloadTaskList)
            {
                if (item.ClearRecordCommand.CanExecute())
                    item.ClearRecordCommand.Execute();
            }
        }


        private void OnDownloadCompleted(DownloadStateChangedEventArgs e)
        {
            var temp = _netDiskUser.GetCompletedFiles().FirstOrDefault(element => element.FileId == e.FileId);
            if (temp == null) return;
            var localFile = Container.Resolve<DownloadedTaskItemViewModel>(new DependencyOverride<ILocalDiskFile>(temp));
            DownloadTaskList.Insert(0, localFile);
        }
    }
}
