using System;
using System.Collections.Generic;

namespace Accelerider.Windows.TransferService
{
    public interface ITransporter<out T> : IObservable<TransferNotification>, IDisposable
    {
        Guid Id { get; }

        T Context { get; }

        TransferStatus Status { get; }

        IReadOnlyDictionary<long, BlockTransferContext> BlockContexts { get; }

        void Run();

        void Stop();
    }

    public interface IDownloader : ITransporter<DownloadContext>
    {
    }
}
