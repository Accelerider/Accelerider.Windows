using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    /// <summary>
    /// Represents an transport engine that is used to manage the life cycle of the <see cref="ITransporter"/> and <see cref="ITransporterBuilder{T}"/> instance. 
    /// </summary>
    public interface ITransferService
    {
        /// <summary>
        /// Initializes an instance of <see cref="ITransferService"/> with the specified <see cref="IConfigureFile"/> instance.
        /// </summary>
        /// <param name="configFile">The specified <see cref="IConfigureFile"/> instance.</param>
        /// <returns>The current instance.</returns>
        ITransferService Initialize(IConfigureFile configFile);

        /// <summary>
        /// Configures the <see cref="TransporterSettings"/> instance with a delegate method.
        /// </summary>
        /// <param name="settings">The delegate method that is used to expose a <see cref="TransporterSettings"/> instance. </param>
        /// <returns>Returns the current instance. </returns>
        ITransferService Configure(Action<TransporterSettings> settings);

        /// <summary>
        /// Destroy a <see cref="ITransferService"/> instance, which will pause all uncompleted tasks 
        /// and returns a <see cref="IConfigureFile"/> instance for persisting status.
        /// </summary>
        /// <returns>An instance of <see cref="IConfigureFile"/> that records the status of the current <see cref="ITransferService"/> instance. </returns>
        IConfigureFile Shutdown();

        /// <summary>
        /// Gets a <see cref="ITransporterBuilder{T}"/> instance with the specified type.
        /// </summary>
        /// <typeparam name="T">The specified type. </typeparam>
        /// <returns>An instance of the specified type. </returns>
        T Use<T>() where T : ITransporterBuilder<ITransporter>;

        /// <summary>
        /// Gets all transporters with the specified task type.
        /// </summary>
        /// <typeparam name="T">The specified task type, which is <see cref="IDownloader"/> or <see cref="IUploader"/>.</typeparam>
        /// <returns>A sequence of download or upload task.</returns>
        IEnumerable<T> GetAll<T>() where T : ITransporter;

        ITransferCommand Command(TransporterToken token);
    }
}
