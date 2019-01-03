using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Threading;
using Prism.Mvvm;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.TransferService.WpfInteractions
{
    public class BindableDownloader : BindableBase
    {
        private const long SampleIntervalBasedMilliseconds = 1000;

        private readonly object _lockObject = new object();

        private TransferStatus _status;

        public Guid Id { get; }

        public TransferStatus Status
        {
            get => _status;
            private set => SetProperty(ref _status, value);
        }

        public BindableProgress Progress { get; }

        public ObservableCollection<BindableBlockTransferItem> BlockDownloadItems { get; }

        internal BindableDownloader(ITransferInfo<DownloadContext> downloader, Dispatcher dispatcher)
        {
            Id = downloader.Id;
            Status = downloader.Status;
            Progress = new BindableProgress();
            BlockDownloadItems = new ObservableCollection<BindableBlockTransferItem>();

            SubscribesDownloader(downloader, dispatcher);
        }

        private void SubscribesDownloader(ITransferInfo<DownloadContext> downloader, Dispatcher dispatcher)
        {
            var observable = dispatcher != null ? downloader.ObserveOn(dispatcher) : downloader;

            // Updates the Status.
            observable
                .Where(item => Status != item.Status)
                .Subscribe(item => Status = item.Status);

            // Initializes this BindableDownloader.
            observable
                .Distinct(item => item.Status)
                .Where(item => item.Status == TransferStatus.Transferring)
                .Subscribe(item =>
                {
                    Progress.TotalSize = downloader.GetTotalSize();

                    var notifiers = downloader.BlockContexts.Values.Select(blockItem =>
                    {
                        var block = new BindableBlockTransferItem(blockItem.Offset);
                        block.Progress.CompletedSize = blockItem.CompletedSize;
                        block.Progress.TotalSize = blockItem.TotalSize;
                        return block;
                    });

                    lock (_lockObject)
                    {
                        BlockDownloadItems.Clear();
                        BlockDownloadItems.AddRange(notifiers);
                    }
                });

            // Updates the progress.
            observable
                .Where(item => item.Status == TransferStatus.Transferring)
                .Sample(TimeSpan.FromMilliseconds(SampleIntervalBasedMilliseconds))
                .Subscribe(item =>
                {
                    Progress.CompletedSize = downloader.GetCompletedSize();

                    lock (_lockObject)
                    {
                        var block = BlockDownloadItems.FirstOrDefault(blockItem => blockItem.Offset == item.Offset);
                        if (block != null)
                        {
                            block.Progress.CompletedSize += item.Bytes;
                        }
                    }
                });
        }
    }
}
