using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.TransferService;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        public ICommand PauseAllCommand { get; }

        public ICommand CancelAllCommand { get; }

        public DownloadingViewModel(IUnityContainer container) : base(container)
        {
            PauseAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => PauseCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);

            CancelAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => CancelCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);

            EventAggregator.GetEvent<TransferItemsAddedEvent>().Subscribe(
                InitializeTransferItem,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: true,
                filter: _ => TransferTasks != null);
        }

        public override void OnLoaded()
        {
            if (TransferTasks == null)
            {
                TransferTasks = new ObservableSortedCollection<IDownloadingFile>((x, y) => x.Downloader.Status - y.Downloader.Status);

                AcceleriderUser
                    .GetDownloadItems()
                    .ForEach(item =>
                    {
                        if (item.Downloader.Status == TransferStatus.Completed)
                        {
                            EventAggregator.GetEvent<TransferItemCompletedEvent>().Publish(item.Downloader.Context.LocalPath.ToTransferredFile());
                        }
                        else
                        {
                            InitializeTransferItem(item);
                        }
                    });
            }
        }

        private void InitializeTransferItem(IDownloadingFile item)
        {
            item.Downloader
                .Where(notification => notification.Status == TransferStatus.Disposed)
                .ObserveOn(Dispatcher)
                .Subscribe(notification => TransferTasks.Remove(item), () =>
                {
                    TransferTasks.Remove(item);
                    EventAggregator.GetEvent<TransferItemCompletedEvent>().Publish(item.Downloader.Context.LocalPath.ToTransferredFile());
                });

            TransferTasks.Add(item);
        }
    }
}
