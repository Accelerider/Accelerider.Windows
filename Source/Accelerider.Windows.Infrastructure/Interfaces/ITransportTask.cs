using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public delegate void StatusChangedEventHandler(ITransportTask sender, TransportStatus status);

    /// <summary>
    /// Represents a download or upload task. 
    /// </summary>
    public interface ITransportTask : IEquatable<ITransportTask>
    {
        /// <summary>
        /// Occurs when the task status changed.
        /// </summary>
        event StatusChangedEventHandler StatusChanged;

        /// <summary>
        /// Gets a <see cref="bool"/> value indicating whether the current task is canceled
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets the current status of the task.
        /// </summary>
        TransportStatus Status { get; }

        /// <summary>
        /// Gets the size of the currently completed data.
        /// </summary>
        DataSize CompletedSize { get; }

        /// <summary>
        /// Gets the size of total data.
        /// </summary>
        DataSize TotalSize { get; }

        /// <summary>
        /// Gets the path of local file, it represents upload source path or download destination path.
        /// </summary>
        FileLocation LocalPath { get; }

        Task StartAsync();

        Task SuspendAsync();

        Task RestartAsync();

        Task CancelAsync();
    }

    public interface IDownloadTask : ITransportTask { }

    public interface IUploadTask : ITransportTask { }
}
