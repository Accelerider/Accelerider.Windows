using System;
using System.Collections.Generic;
using Accelerider.Windows.TransferService;
using Accelerider.Windows.TransferService.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public class TransferItem : IDownloader
    {
        private readonly IDownloader _downloader;

        public INetDiskUser Owner { get; set; }

        public INetDiskFile File { get; set; }

        public BindableDownloader Transporter { get; }

        public IManagedTransporterToken ManagedToken { get; }


        public TransferItem(IDownloader downloader)
        {
            _downloader = downloader;
            ManagedToken = this.AsManaged();
            Transporter = _downloader.ToBindable();
        }


        #region Proxy IDownloader interface

        public Guid Id => _downloader.Id;

        public DownloadContext Context => _downloader.Context;

        public TransferStatus Status => _downloader.Status;

        public IReadOnlyDictionary<Guid, BlockTransferContext> BlockContexts => _downloader.BlockContexts;

        public object Tag
        {
            get => _downloader.Tag;
            set => _downloader.Tag = value;
        }

        public void Run() => _downloader.Run();

        public void Stop() => _downloader.Stop();

        public IDisposable Subscribe(IObserver<TransferNotification> observer) => _downloader.Subscribe(observer);

        public void Dispose() => _downloader.Dispose();

        #endregion
    }
}