using System.Collections.Generic;
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
        /// <returns>A sequence of <see cref="ITransportTask"/> type.</returns>
        IEnumerable<ITransportTask> FindAll();

        /// <summary>
        /// Gets all tasks in the specified status.
        /// </summary>
        /// <param name="status">The specified task status.</param>
        /// <returns>A sequence of <see cref="ITransportTask"/> type.</returns>
        IEnumerable<ITransportTask> FindAll(TransportStatus status);
    }
}
