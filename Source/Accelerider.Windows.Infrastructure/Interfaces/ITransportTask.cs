using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Exceptions;
using Accelerider.Windows.Infrastructure.Extensions;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public delegate void StatusChangedEventHandler(ITransportTask sender, StatusChangedEventArgs e);

    public class StatusChangedEventArgs
    {
        public StatusChangedEventArgs(TransportStatus oldStatus, TransportStatus newStatus)
        {
            if (!oldStatus.CanConvertedTo(newStatus))
                throw new InvalidTransportStatusTransitionException(oldStatus, newStatus);

            OldStatus = oldStatus;
            NewStatus = newStatus;
        }

        public TransportStatus OldStatus { get; }

        public TransportStatus NewStatus { get; }
    }

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
        /// Gets a <see cref="bool"/> value indicating whether the current task is disposed.
        /// </summary>
        bool IsDisposed { get; }

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

        /// <summary>
        /// Force start or restart the task, which may lead to transfer 
        /// from <see cref="TransportStatus.Suspended"/> / <see cref="TransportStatus.Faulted"/> / 
        /// <see cref="TransportStatus.Ready"/> to <see cref="TransportStatus.Transporting"/>. 
        /// </summary>
        /// <returns>A <see cref="Task"/> instance for waiting. </returns>
        Task StartAsync();

        /// <summary>
        /// Suspends the task, which may lead to transfer 
        /// from <see cref="TransportStatus.Ready"/> / <see cref="TransportStatus.Transporting"/> to <see cref="TransportStatus.Suspended"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance for waiting. </returns>
        Task SuspendAsync();

        /// <summary>
        /// Dispose the task, any status can be disposed.
        /// If the task is in the <see cref="TransportStatus.Transporting"/>, it will be suspended first and then disposed.
        /// </summary>
        /// <returns>A <see cref="Task"/> instance for waiting. </returns>
        Task DisposeAsync();
    }

    public interface IDownloadTask : ITransportTask { }

    public interface IUploadTask : ITransportTask { }
}
