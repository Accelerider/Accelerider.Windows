using System;
using System.Collections.Generic;
using System.Net;   

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public delegate T Interceptor<T>(T input, TransferContext context);

    /// <summary>
    /// Represents an download or upload task builder.
    /// </summary>
    public interface IDownloaderBuilder
    {
        /// <summary>
        /// Sets a uri that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloaderBuilder From(string path);

        /// <summary>
        /// Sets a uri sequence that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloaderBuilder From(IEnumerable<string> paths);

        /// <summary>
        /// Sets a uri that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloaderBuilder To(string path);

        IDownloaderBuilder Configure(Action<TransferSettings, TransferContext> settingsConfigurator);

        IDownloaderBuilder Configure(Func<IEnumerable<string>, IRemotePathProvider> remotePathProviderBuilder);

        IDownloaderBuilder Configure(Func<HttpWebRequest, HttpWebRequest> requestInterceptor);

        IDownloaderBuilder Configure(Func<string, string> localPathInterceptor);

        IDownloaderBuilder Configure(Func<long, IEnumerable<(long offset, long length)>> blockIntervalGenerator);

        IDownloaderBuilder Configure(Interceptor<IObservable<BlockTransferContext>> blockTransferItemInterceptor);

        /// <summary>
        /// Get a snapshot copy of the current instance. 
        /// </summary>
        /// <returns>Returns a snapshot copy of the current instance. </returns>
        IDownloaderBuilder Clone();

        /// <summary>
        /// Build a <see cref="IDownloader"/> instance based on the current configuration.
        /// </summary>
        /// <returns>An instance of the <see cref="IDownloader"/> derived class.</returns>
        IDownloader Build();
    }
}
