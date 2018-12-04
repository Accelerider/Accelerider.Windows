using System;
using System.Collections.Generic;
using System.Net;

namespace Accelerider.Windows.TransferService
{
    /// <summary>
    /// Represents an download or upload task builder.
    /// </summary>
    public interface IDownloaderBuilder
    {
        IDownloaderBuilder Configure(Action<DownloadSettings, DownloadContext> settingsConfigurator);

        IDownloaderBuilder Configure(Func<HttpWebRequest, HttpWebRequest> requestInterceptor);

        IDownloaderBuilder Configure(Func<string, string> localPathInterceptor);

        IDownloaderBuilder Configure(Func<long, IEnumerable<(long Offset, long Length)>> blockIntervalGenerator);

        /// <summary>
        /// Sets a <see cref="IRemotePathProvider"/> instance to provide the remote path.
        /// </summary>
        /// <param name="provider">The <see cref="IRemotePathProvider"/> instance</param>
        /// <returns>Returns the current instance. </returns>
        IDownloaderBuilder From(IRemotePathProvider provider);

        /// <summary>
        /// Sets a uri that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        IDownloaderBuilder To(string path);

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

        IDownloader Build(string json);
    }
}
