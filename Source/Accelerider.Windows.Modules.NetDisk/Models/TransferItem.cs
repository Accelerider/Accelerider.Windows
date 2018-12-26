using System;
using System.Collections.Generic;
using Accelerider.Windows.TransferService;
using Accelerider.Windows.TransferService.WpfInteractions;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public class TransferItem : IDownloader
    {
        public IDownloader Transporter { get; }

        public INetDiskUser Owner { get; set; }

        public INetDiskFile File { get; set; }

        public BindableDownloader BindableTransporter { get; }

        public IManagedTransporterToken ManagedToken { get; }

        public TransferItem(IDownloader downloader, string managerName = FileTransferService.DefaultManagerName)
        {
            Transporter = downloader;
            ManagedToken = this.AsManaged(managerName);
            BindableTransporter = Transporter.ToBindable();
        }

        #region Proxy IDownloader interface

        public Guid Id => Transporter.Id;

        public DownloadContext Context => Transporter.Context;

        public TransferStatus Status => Transporter.Status;

        public IReadOnlyDictionary<long, BlockTransferContext> BlockContexts => Transporter.BlockContexts;

        public void Run() => Transporter.Run();

        public void Stop() => Transporter.Stop();

        public IDisposable Subscribe(IObserver<TransferNotification> observer) => Transporter.Subscribe(observer);

        public void Dispose() => Transporter.Dispose();

        #endregion
    }
}