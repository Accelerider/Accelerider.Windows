using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    /// <summary>
    /// Represents an download or upload task builder.
    /// </summary>
    public interface ITransporterBuilder
    {
        /// <summary>
        /// Sets a uri that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder From(string path);

        /// <summary>
        /// Sets a uri sequence that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder From(IEnumerable<string> paths);

        /// <summary>
        /// Sets a uri that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder To(string path);

        ITransporterBuilder Configure(Action<TransferSettings, TransferContext> settings);

        ITransporterBuilder Configure(Func<HttpWebRequest, HttpWebRequest> requestInterceptor);

        ITransporterBuilder Configure(Func<string, string> localPathInterceptor);

        ITransporterBuilder Configure(Func<long, IEnumerable<(long offset, long length)>> blockGenerator);

        ITransporterBuilder Configure(Func<IEnumerable<string>, IRemotePathProvider> remotePathProviderBuilder);

        /// <summary>
        /// Get a snapshot copy of the current instance. 
        /// </summary>
        /// <returns>Returns a snapshot copy of the current instance. </returns>
        ITransporterBuilder Clone();

        /// <summary>
        /// Build a <see cref="ITransporter"/> instance based on the current configuration.
        /// </summary>
        /// <returns>An instance of the <see cref="ITransporter"/> derived class.</returns>
        Task<ITransporter> BuildAsync(CancellationToken cancellationToken);
    }
}
