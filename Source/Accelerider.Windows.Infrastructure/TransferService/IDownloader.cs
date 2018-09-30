using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IDownloader : IObservable<BlockTransferContext>, IDisposable, INotifyPropertyChanged, IJsonable
    {
        TransferStatus Status { get; }

        DownloadContext Context { get; }

        IReadOnlyCollection<BlockTransferContext> BlockContexts { get; }

        /// <summary>
        /// Sets a uri that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloader From(string path);

        /// <summary>
        /// Sets a uri sequence that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloader From(IEnumerable<string> paths);

        /// <summary>
        /// Sets a uri that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloader To(string path);

        Task ActivateAsync(CancellationToken cancellationToken = default);

        void Suspend();
    }

    public static class DownloaderExtensions
    {
        public static long GetCompletedSize(this IDownloader @this)
        {
            return @this.BlockContexts.Sum(item => item.CompletedSize);
        }
    }
}
