using System;
using System.Collections.ObjectModel;
using System.Linq;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.ViewModels.Items;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : ViewModelBase
    {
        private ObservableCollection<TransferTaskViewModel> _downloadTasks;


        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
            DownloadTasks = new ObservableCollection<TransferTaskViewModel>(AcceleriderUser.GetDownloadingFiles().Select(item =>
            {
                item.TransferStateChanged += PublishTransferStateChanged;
                return new TransferTaskViewModel(item);
            }));
            EventAggregator.GetEvent<TransferStateChangedEvent>().Subscribe(OnTransferStateChanged, e => e.NewState == TransferStateEnum.Checking);
        }

        private void OnTransferStateChanged(TransferStateChangedEventArgs e)
        {
            var temp = DownloadTasks.FirstOrDefault(item => item.FileInfo.FilePath.FullPath == e.Token.FileInfo.FilePath.FullPath);
            if (temp != null)
            {
                DownloadTasks.Remove(temp);
                GlobalMessageQueue.Enqueue($"\"{e.Token.FileInfo.FilePath.FileName}\" has been downloaded.");
            }
        }

        public ObservableCollection<TransferTaskViewModel> DownloadTasks
        {
            get => _downloadTasks;
            set => SetProperty(ref _downloadTasks, value);
        }

        private void PublishTransferStateChanged(object sender, TransferStateChangedEventArgs e)
        {
            EventAggregator.GetEvent<TransferStateChangedEvent>().Publish(e);
        }
    }
}
