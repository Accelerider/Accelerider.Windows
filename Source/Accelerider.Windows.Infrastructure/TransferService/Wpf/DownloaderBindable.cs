using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace Accelerider.Windows.Infrastructure.TransferService.Wpf
{
    public class DownloaderBindable : BindableBase
    {
        private const long SampleIntervalBasedMilliseconds = 1000;

        private Guid _id;
        private TransferStatus _status;
        private ObservableCollection<DownloaderBlockItemBindable> _blockNotifiers;

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

        public ProgressBindable Progress { get; }

        public ObservableCollection<DownloaderBlockItemBindable> BlockNotifiers
        {
            get => _blockNotifiers;
            set => SetProperty(ref _blockNotifiers, value);
        }

        internal DownloaderBindable(IDownloader downloader, Dispatcher dispatcher)
        {
            Status = downloader.Status;
            if (downloader.Context != null)
            {
                Id = downloader.Context.Id;
                Progress = new ProgressBindable(SampleIntervalBasedMilliseconds)
                {
                    CompletedSize = downloader.GetCompletedSize(),
                    TotalSize = downloader.GetTotalSize()
                };
            }

            BlockNotifiers = new ObservableCollection<DownloaderBlockItemBindable>();
            if (downloader.BlockContexts != null)
            {
                var notifiers = downloader.BlockContexts.Values.Select(item =>
                {
                    var result = new DownloaderBlockItemBindable(item.Id, SampleIntervalBasedMilliseconds);
                    result.Progress.CompletedSize = item.CompletedSize;
                    result.Progress.TotalSize = item.TotalSize;
                    return result;
                });
                BlockNotifiers.AddRange(notifiers);
            }

            downloader
                .ObserveOn(dispatcher ?? Dispatcher.CurrentDispatcher)
                .Sample(TimeSpan.FromMilliseconds(SampleIntervalBasedMilliseconds))
                .Subscribe(item =>
                {
                    Status = item.Status;
                    Progress.CompletedSize = downloader.GetCompletedSize();
                    Progress.TotalSize = downloader.GetTotalSize();

                    var block = BlockNotifiers.FirstOrDefault(blockItem => blockItem.Id == item.CurrentBlockId);
                    if (block != null)
                    {
                        block.Progress.CompletedSize += item.Bytes;
                        block.Progress.TotalSize = downloader.BlockContexts[item.CurrentBlockId].TotalSize;
                    }
                });
        }
    }
}
