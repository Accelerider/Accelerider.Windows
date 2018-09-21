using System;
using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    /// <summary>
    /// Represents an transport engine that is used to manage the life cycle of the <see cref="ITransporter"/> and <see cref="ITransporterBuilder{T}"/> instance. 
    /// </summary>
    public interface ITransferService
    {
        IEnumerable<IDownloader> Downloaders { get; }

        IEnumerable<IUploader> Uploaders { get; }

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

        void Run();

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

        ITransporterRegistry Register(ITransporter transporter);
    }
}
