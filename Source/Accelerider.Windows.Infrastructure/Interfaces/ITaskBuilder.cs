using System;
using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents an download or upload task builder.
    /// </summary>
    public interface ITaskBuilder
    {
        /// <summary>
        /// Sets a uri that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        ITaskBuilder From(string path);

        /// <summary>
        /// Sets a uri sequence that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        ITaskBuilder From(IEnumerable<string> paths);

        /// <summary>
        /// Sets a uri that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        ITaskBuilder To(string path);

        /// <summary>
        /// Sets a uri sequence that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        ITaskBuilder To(IEnumerable<string> paths);

        /// <summary>
        /// Configures the <see cref="TransportSettings"/> instance with a delegate method.
        /// </summary>
        /// <param name="settings">The delegate method that is used to expose a <see cref="TransportSettings"/> instance. </param>
        /// <returns>Returns the current instance. </returns>
        ITaskBuilder Configure(Action<TransportSettings> settings);

        /// <summary>
        /// Get a snapshot copy of the current instance. 
        /// </summary>
        /// <returns>Returns a snapshot copy of the current instance. </returns>
        ITaskBuilder Clone();
    }

    public interface IDownloadTaskBuilder : ITaskBuilder
    {
        IDownloadTask Build();

        IDownloadTask Update(IDownloadTask task);
    }

    public interface IUploadTaskBuilder : ITaskBuilder
    {
        IUploadTask Build();

        IUploadTask Update(IUploadTask task);
    }
}
