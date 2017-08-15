using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
            DownloadTasks = new ObservableCollection<TransferTaskViewModel>(NetDiskUser.GetDownloadingFiles().Select(item =>
            {
                item.TransferStateChanged += OnDownloaded;
                return new TransferTaskViewModel(item);
            }));

            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Subscribe(OnDownloadTaskCreated, token => token != null && token.Any());
        }

        protected override Task LoadAsync()
        {
            return base.LoadAsync();
        }

        private void OnDownloadTaskCreated(IReadOnlyCollection<ITransferTaskToken> tokens)
        {
            foreach (var token in tokens)
            {
                DownloadTasks.Add(new TransferTaskViewModel(token));
            }
        }

        public ObservableCollection<TransferTaskViewModel> DownloadTasks
        {
            get => _downloadTasks;
            set => SetProperty(ref _downloadTasks, value);
        }

        private void OnDownloaded(object sender, TransferStateChangedEventArgs e)
        {
            if (e.NewState != TransferStateEnum.Checking) return;

            var temp = DownloadTasks.FirstOrDefault(item => item.FileInfo.FilePath.FullPath == e.Token.FileInfo.FilePath.FullPath);
            if (temp != null)
            {
                DownloadTasks.Remove(temp);
                GlobalMessageQueue.Enqueue($"\"{e.Token.FileInfo.FilePath.FileName}\" has been downloaded.");
            }
            EventAggregator.GetEvent<TransferStateChangedEvent>().Publish(e);
        }
    }
}
