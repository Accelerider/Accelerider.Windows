using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface ITransporter : IObservable<TransferNotification>, IDisposable, IJsonable<IDownloader>
    {
        Guid Id { get; }

        TransferStatus Status { get; }

        IReadOnlyDictionary<Guid, BlockTransferContext> BlockContexts { get; }

        object Tag { get; set; }

        /// <summary>
        /// Sets a uri that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloader From(string path);

        /// <summary>
        /// Sets a uri that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloader To(string path);

        Task ActivateAsync(CancellationToken cancellationToken = default);

        void Suspend();
    }
}
