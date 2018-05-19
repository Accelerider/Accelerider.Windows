using System;
using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    /// <summary>
    /// Represents an download or upload task builder.
    /// </summary>
    public interface ITransporterBuilder<out T> where T : ITransporter
    {
        /// <summary>
        /// Sets a uri that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder<T> From(string path);

        /// <summary>
        /// Sets a uri sequence that represents the file source, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file source. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder<T> From(IEnumerable<string> paths);

        /// <summary>
        /// Sets a uri that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="path">The uri that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder<T> To(string path);

        /// <summary>
        /// Sets a uri sequence that represents the file destination, it can be url or local disk path.
        /// </summary>
        /// <param name="paths">The uri sequence that represents the file destination. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder<T> To(IEnumerable<string> paths);

        /// <summary>
        /// Configures the <see cref="TransporterSettings"/> instance with a delegate method.
        /// </summary>
        /// <param name="settings">The delegate method that is used to expose a <see cref="TransporterSettings"/> instance. </param>
        /// <returns>Returns the current instance. </returns>
        ITransporterBuilder<T> Configure(Action<TransporterSettings> settings);

        /// <summary>
        /// Get a snapshot copy of the current instance. 
        /// </summary>
        /// <returns>Returns a snapshot copy of the current instance. </returns>
        ITransporterBuilder<T> Clone();

        /// <summary>
        /// Build a <see cref="ITransporter"/> instance based on the current configuration.
        /// </summary>
        /// <returns>An instance of the <see cref="ITransporter"/> derived class.</returns>
        T Build();
    }
}
