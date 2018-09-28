using System;
using System.Collections.Generic;
using System.Net;   

namespace Accelerider.Windows.Infrastructure.TransferService
{
    /// <summary>
    /// Represents an download or upload task builder.
    /// </summary>
    public interface IDownloaderBuilder
    {
        IDownloaderBuilder Configure(Action<TransferSettings, TransferContext> settingsConfigurator);

        IDownloaderBuilder Configure(Func<IEnumerable<string>, IRemotePathProvider> remotePathProviderBuilder);

        IDownloaderBuilder Configure(Func<HttpWebRequest, HttpWebRequest> requestInterceptor);

        IDownloaderBuilder Configure(Func<string, string> localPathInterceptor);

        IDownloaderBuilder Configure(Func<long, IEnumerable<(long offset, long length)>> blockIntervalGenerator);

        IDownloaderBuilder Configure(Func<IObservable<BlockTransferContext>, IObservable<BlockTransferContext>> blockTransferItemInterceptor);

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
