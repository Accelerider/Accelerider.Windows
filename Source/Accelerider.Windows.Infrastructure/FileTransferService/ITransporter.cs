using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a download or upload task, and it should not be implemented.
    /// </summary>
    public interface ITransporter
    {
        /// <summary>
        /// Occurs when the task status changed.
        /// </summary>
        event EventHandler<TransferStatusChangedEventArgs> StatusChanged;

        TransporterId Id { get; }

        /// <summary>
        /// Gets the current status of the task.
        /// </summary>
        TransferStatus Status { get; }

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
        FileLocator LocalPath { get; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents a download task flag.
    /// </summary>
    public interface IDownloader : ITransporter { }

    /// <inheritdoc />
    /// <summary>
    /// Represents a upload task flag.
    /// </summary>
    public interface IUploader : ITransporter { }
}
