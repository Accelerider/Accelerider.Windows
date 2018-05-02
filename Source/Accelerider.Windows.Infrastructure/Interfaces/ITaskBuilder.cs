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

        /// <summary>
        /// Build a <see cref="ITransportTask"/> instance based on the current configuration. 
        /// </summary>
        /// <returns>Returns a <see cref="ITransportTask"/> instance. </returns>
        ITransportTask Build();

        /// <summary>
        /// Update an existing <see cref="ITransportTask"/> instance based on the current configuration. 
        /// </summary>
        /// <param name="task">The existing <see cref="ITransportTask"/> instance. </param>
        /// <returns>Returns the updated <see cref="ITransportTask"/> instance. </returns>
        ITransportTask Update(ITransportTask task);
    }

    //public static class ITaskBuilderExtensions
    //{
    //    public static IDownloadTask Update(this ITaskBuilder<IDownloadTask> @this, IDownloadTask task)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static IUploadTask Update(this ITaskBuilder<IUploadTask> @this, IUploadTask task)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
