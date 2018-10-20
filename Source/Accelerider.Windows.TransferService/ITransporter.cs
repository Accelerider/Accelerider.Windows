using System;
using System.Collections.Generic;

namespace Accelerider.Windows.TransferService
{
    public interface ITransporter<out T> : IObservable<TransferNotification>, IDisposable where T : TransferContextBase
    {
        Guid Id { get; }

        T Context { get; }

        TransferStatus Status { get; }

        IReadOnlyDictionary<Guid, BlockTransferContext> BlockContexts { get; }

        object Tag { get; set; }

        void Run();

        void Stop();
    }

    public interface IDownloader : ITransporter<DownloadContext>
    {
    }
}
