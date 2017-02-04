using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Infrastructure.Events;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.ViewModels.Items;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal class DownloadedPageViewModel : ViewModelBase
    {
        private readonly ILocalDiskUser _localDiskUser;
        private ObservableCollection<DownloadedTaskItemViewModel> _downloadTaskList = new ObservableCollection<DownloadedTaskItemViewModel>();

        public DownloadedPageViewModel(IUnityContainer container, ILocalDiskUserRepository localDiskUserRepository)
            : base(container)
        {
            _localDiskUser = localDiskUserRepository.FirstOrDefault();
            //var temp = _localDiskUser?.GetAllLocalDiskFiles();
            //if (temp != null)
            //{
            //    foreach (var file in temp)
            //    {
            //        DownloadTaskList.Add(Container.Resolve<DownloadedTaskItemViewModel>(new DependencyOverride<IDiskFile>(file)));
            //    }
            //}

            ClearAllRecordCommand = new Command(ClearAllRecordCommandExecute, () => DownloadTaskList?.Any() ?? false);
            EventAggregator.GetEvent<DownloadStateChangedEvent>().Subscribe(OnDownloadCompleted,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: false,
                filter: e => e.NewState == DownloadStateEnum.Completed);
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
            //var temp = _localDiskUser.GetLocalDiskFileById(e.FileId);
            //if (temp == null) return;
            //var localFile = Container.Resolve<DownloadedTaskItemViewModel>(new DependencyOverride<IDiskFile>(temp));
            //localFile.CompletedTime = e.Timestamp.ToString("yyyy-MM-dd HH:mm");
            //DownloadTaskList.Insert(0, localFile);
        }
    }
}
