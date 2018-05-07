using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents an transport engine that is used to manage the life cycle of the <see cref="ITransportTask"/> and <see cref="ITaskBuilder"/> instance. 
    /// </summary>
    public interface ITransportContainer
    {
        /// <summary>
        /// Initializes an instance of <see cref="ITransportContainer"/> with the specified <see cref="IConfigureFile"/> instance.
        /// </summary>
        /// <param name="configFile">The specified <see cref="IConfigureFile"/> instance.</param>
        /// <returns>The current instance.</returns>
        ITransportContainer Initialize(IConfigureFile configFile);

        /// <summary>
        /// Configures the <see cref="TransportSettings"/> instance with a delegate method.
        /// </summary>
        /// <param name="settings">The delegate method that is used to expose a <see cref="TransportSettings"/> instance. </param>
        /// <returns>Returns the current instance. </returns>
        ITransportContainer Configure(Action<TransportSettings> settings);

        /// <summary>
        /// Destroy a <see cref="ITransportContainer"/> instance, which will pause all uncompleted tasks 
        /// and returns a <see cref="IConfigureFile"/> instance for persisting status.
        /// </summary>
        /// <returns>An instance of <see cref="IConfigureFile"/> that records the status of the current <see cref="ITransportContainer"/> instance. </returns>
        Task<IConfigureFile> ShutdownAsync();

        /// <summary>
        /// Gets a <see cref="ITaskBuilder"/> instance with the specified type.
        /// </summary>
        /// <typeparam name="T">The specified type. </typeparam>
        /// <returns>An instance of the specified type. </returns>
        T Use<T>() where T : ITaskBuilder;

        /// <summary>
        /// Gets all tasks in all status.
        /// </summary>
        /// <typeparam name="T">The specified task type, which is <see cref="IDownloadTask"/> or <see cref="IUploadTask"/>.</typeparam>
        /// <returns>A sequence of download or upload task.</returns>
        IEnumerable<T> GetAllTasks<T>() where T : ITransportTask;
    }
}
