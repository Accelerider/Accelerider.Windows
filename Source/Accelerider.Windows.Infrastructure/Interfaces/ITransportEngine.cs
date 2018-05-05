using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents an transport engine that is used to manage the life cycle of the <see cref="ITransportTask"/> and <see cref="ITaskBuilder"/> instance. 
    /// </summary>
    public interface ITransportEngine
    {
        /// <summary>
        /// Initializes an instance of <see cref="ITransportEngine"/> with the specified configure file path.
        /// </summary>
        /// <param name="configFilePath">The specified configure file path.</param>
        Task InitializeAsync(string configFilePath);

        /// <summary>
        /// Initializes an instance of <see cref="ITransportEngine"/> with the specified <see cref="IConfigureFile"/> instance.
        /// </summary>
        /// <param name="configFile">The specified <see cref="IConfigureFile"/> instance.</param>
        Task InitializeAsync(IConfigureFile configFile);

        /// <summary>
        /// Destroy a <see cref="ITransportEngine"/> instance, which will pause all uncompleted tasks 
        /// and returns a <see cref="IConfigureFile"/> instance for persisting status.
        /// </summary>
        /// <returns>An instance of <see cref="IConfigureFile"/> that records the status of the current <see cref="ITransportEngine"/> instance. </returns>
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
        IEnumerable<T> FindAll<T>() where T : ITransportTask;


        /// <summary>
        /// Gets all tasks with the specified predicate.
        /// </summary>
        /// <typeparam name="T">The specified task type, which is <see cref="IDownloadTask"/> or <see cref="IUploadTask"/>.</typeparam>
        /// <param name="predicate">A function to test each element for a condition. </param>
        /// <returns>A sequence of download or upload task that passes the test in the specified predicate function.</returns>
        IEnumerable<T> FindAll<T>(Func<T, bool> predicate) where T : ITransportTask;
    }
}
