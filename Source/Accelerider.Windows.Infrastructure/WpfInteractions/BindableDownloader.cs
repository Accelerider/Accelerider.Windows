using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Threading;
using Accelerider.Windows.Infrastructure.TransferService;
using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure.WpfInteractions
{
    public class BindableDownloader : BindableBase
    {
        private const long SampleIntervalBasedMilliseconds = 1000;

        private Guid _id;
        private TransferStatus _status;
        private ObservableCollection<BindableBlockDownloadItem> _blockDownloadItems;

        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public TransferStatus Status
        {
            get => _status;
            internal set => SetProperty(ref _status, value);
        }

        public BindableProgress Progress { get; }

        public ObservableCollection<BindableBlockDownloadItem> BlockDownloadItems
        {
            get => _blockDownloadItems;
            set => SetProperty(ref _blockDownloadItems, value);
        }

        internal BindableDownloader(IDownloader downloader, Dispatcher dispatcher)
        {
            Status = downloader.Status;
            Progress = new BindableProgress(SampleIntervalBasedMilliseconds);
            BlockDownloadItems = new ObservableCollection<BindableBlockDownloadItem>();

            SubscribesDownloader(downloader, dispatcher);
        }

        private void SubscribesDownloader(IDownloader downloader, Dispatcher dispatcher)
        {
            var observable = downloader.ObserveOn(dispatcher);

            var observableStatus = observable.Distinct(item => item.Status);

            observableStatus
                .Where(item => item.Status == TransferStatus.Transferring)
                .Subscribe(item =>
                {
                    Id = downloader.Context.Id;
                    var notifiers = downloader.BlockContexts.Values.Select(blockItem =>
                    {
                        var result = new BindableBlockDownloadItem(blockItem.Id, SampleIntervalBasedMilliseconds);
                        result.Progress.CompletedSize = blockItem.CompletedSize;
                        result.Progress.TotalSize = blockItem.TotalSize;
                        return result;
                    });
                    BlockDownloadItems.Clear();
                    BlockDownloadItems.AddRange(notifiers);
                });

            observableStatus
                .Subscribe(item => Status = item.Status);

            observable
                .Where(item => item.Status == TransferStatus.Transferring)
                .Sample(TimeSpan.FromMilliseconds(SampleIntervalBasedMilliseconds))
                .Subscribe(item =>
                {
                    Progress.CompletedSize = downloader.GetCompletedSize();
                    Progress.TotalSize = downloader.GetTotalSize();

                    var block = BlockDownloadItems.FirstOrDefault(blockItem => blockItem.Id == item.CurrentBlockId);
                    if (block != null)
                    {
                        block.Progress.CompletedSize += item.Bytes;
                        block.Progress.TotalSize = downloader.BlockContexts[item.CurrentBlockId].TotalSize;
                    }
                });
        }
    }
}
