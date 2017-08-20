using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.ViewModels.Items;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : ViewModelBase
    {
        private ObservableCollection<TransferTaskViewModel> _downloadTasks;


        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
            DownloadTasks = new ObservableCollection<TransferTaskViewModel>(AcceleriderUser.GetDownloadingTasks().Select(item =>
            {
                item.TransferStateChanged += OnDownloaded;
                return new TransferTaskViewModel(new TaskCreatedEventArgs(NetDiskUser.Username, item));
            }));

            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Subscribe(OnDownloadTaskCreated, token => token != null && token.Any());
        }


        public ObservableCollection<TransferTaskViewModel> DownloadTasks
        {
            get => _downloadTasks;
            set => SetProperty(ref _downloadTasks, value);
        }


        private void OnDownloadTaskCreated(IReadOnlyCollection<TaskCreatedEventArgs> taskInfos)
        {
            foreach (var taskInfo in taskInfos)
            {
                taskInfo.Token.TransferStateChanged += OnDownloaded;
                DownloadTasks.Add(new TransferTaskViewModel(taskInfo));
            }
        }

        private void OnDownloaded(object sender, TransferStateChangedEventArgs e)
        {
            if (e.NewState != TransferStateEnum.Checking) return;

            var temp = DownloadTasks.FirstOrDefault(item => item.FileInfo.FilePath.FullPath == e.Token.FileInfo.FilePath.FullPath);
            if (temp != null)
            {
                DownloadTasks.Remove(temp);
            }
        }
    }
}
