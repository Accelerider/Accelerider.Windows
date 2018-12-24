using System;
using System.Collections.Generic;
using Accelerider.Windows.TransferService;
using Accelerider.Windows.TransferService.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public class TransferItem : IDownloader
    {
        public IDownloader Downloader { get; }

        public INetDiskUser Owner { get; set; }

        public INetDiskFile File { get; set; }

        public BindableDownloader Transporter { get; }

        public IManagedTransporterToken ManagedToken { get; }

        public TransferItem(IDownloader downloader, string managerName = FileTransferService.DefaultManagerName)
        {
            Downloader = downloader;
            ManagedToken = this.AsManaged(managerName);
            Transporter = Downloader.ToBindable();
        }


        #region Proxy IDownloader interface

        public Guid Id => Downloader.Id;

        public DownloadContext Context => Downloader.Context;

        public TransferStatus Status => Downloader.Status;

        public IReadOnlyDictionary<long, BlockTransferContext> BlockContexts => Downloader.BlockContexts;

        public void Run() => Downloader.Run();

        public void Stop() => Downloader.Stop();

        public IDisposable Subscribe(IObserver<TransferNotification> observer) => Downloader.Subscribe(observer);

        public void Dispose() => Downloader.Dispose();

        #endregion
    }
}