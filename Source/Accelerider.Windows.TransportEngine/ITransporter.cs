using Accelerider.Windows.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.TransportEngine
{
    public interface ITransporter
    {
        // Life cycle management --------------------------------------------------------------------

        /// <summary>
        /// Initializes an instance of <see cref="ITransporter"/> with the specified configure file path.
        /// </summary>
        /// <param name="configFilePath">The specified configure file path.</param>
        Task Initialize(string configFilePath);

        /// <summary>
        /// Initializes an instance of <see cref="ITransporter"/> with the specified <see cref="IConfigureFile"/> instance.
        /// </summary>
        /// <param name="configFile">The specified <see cref="IConfigureFile"/> instance.</param>
        Task Initialize(IConfigureFile configFile);

        /// <summary>
        /// Destory a <see cref="ITransporter"/> instance, which will pause all uncompleted tasks 
        /// and returns a <see cref="IConfigureFile"/> instance for persisting status.
        /// </summary>
        /// <returns>An instance of <see cref="IConfigureFile"/> that records the status of the current <see cref="ITransporter"/> instance. </returns>
        Task<IConfigureFile> Shutdown();

        // Configure management ---------------------------------------------------------------------

        /// <summary>
        /// Gets the information of configure.
        /// </summary>
        IConfigureFile Configure { get; }

        // Tasks management -------------------------------------------------------------------------

        /// <summary>
        /// Cancels a task with the specified reference. 
        /// </summary>
        /// <param name="task">The task reference</param>
        /// <returns>Returns a <see cref="bool"/> value that indicates whether success. </returns>
        Task<bool> Remove(ITaskReference task);

        /// <summary>
        /// Pauses all active task.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> value that indicates whether success. </returns>
        Task<bool> Pause();

        /// <summary>
        /// Pauses a task with the specified reference. 
        /// </summary>
        /// <param name="task">The task reference</param>
        /// <returns>Returns a <see cref="bool"/> value that indicates whether success. </returns>
        Task<bool> Pause(ITaskReference task);

        /// <summary>
        /// Starts all paused task
        /// </summary>
        /// <returns></returns>
        Task<bool> Start();

        /// <summary>
        /// Starts a paused task with the specified reference. 
        /// </summary>
        /// <param name="task">The task reference</param>
        /// <param name="force"></param>
        /// <returns>Returns a <see cref="bool"/> value that indicates whether success. </returns>
        Task<bool> Start(ITaskReference task, bool force = false);

        /// <summary>
        /// Gets all tasks in all status, except for <see cref="TransportStatus.Canceled"/> tasks.
        /// </summary>
        /// <returns>A sequance of <see cref="ITaskReference"/> type.</returns>
        IEnumerable<ITaskReference> FindAll();

        /// <summary>
        /// Gets all tasks in the specified status, cannot get the <see cref="TransportStatus.Canceled"/> task.
        /// </summary>
        /// <param name="status">The specified task status, which cannot be <see cref="TransportStatus.Canceled"/> status.</param>
        /// <returns>A sequance of <see cref="ITaskReference"/> type.</returns>
        IEnumerable<ITaskReference> FindAll(TransportStatus status);
    }
}
