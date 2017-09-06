using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents the summary of the transfer task.
    /// </summary>
    public interface ITransferTaskToken : IEquatable<ITransferTaskToken>
    {
        /// <summary>
        /// Occurs when the <see cref="TaskStatus"/> was changed.
        /// </summary>
        event EventHandler<TransferTaskStatusChangedEventArgs> TransferTaskStatusChanged;

        /// <summary>
        /// Gets the name of the net-disk that is related with this task.
        /// </summary>
        string OwnerName { get; }

        /// <summary>
        /// Gets a <see cref="TransferTaskStatusEnum"/> value that indicates the status of this task.
        /// </summary>
        TransferTaskStatusEnum TaskStatus { get; }

        /// <summary>
        /// Gets a <see cref="DataSize"/> value that indicates the file size that has been completed.
        /// </summary>
        DataSize Progress { get; }

        /// <summary>
        /// Gets a <see cref="IDiskFile"/> object that contains the information of the file that belongs to this task.
        /// </summary>
        IFileSummary FileSummary { get; }

        /// <summary>
        /// Gets the transfered file after this task was completed (<see cref="TransferTaskStatusEnum.Completed"/>).
        /// Returns null when the <see cref="TaskStatus"/> of this task is any other value.
        /// </summary>
        /// <returns>Returns a <see cref="ITransferedFile"/> object that represents the result of this task.</returns>
        ITransferedFile GetTransferedFile();

        /// <summary>
        /// Pauses this task, and keeps the task progress information
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> value that inducates whether the operation was seccessful.</returns>
        Task<bool> PauseAsync();

        /// <summary>
        /// Restarts or forced to start this task.
        /// </summary>
        /// <param name="force">The <see cref="bool"/> value indicates whether the task needs to be forced to start.</param>
        /// <returns>Returns a <see cref="bool"/> value that inducates whether the operation was seccessful.</returns>
        Task<bool> StartAsync(bool force = false);

        /// <summary>
        /// Cancels this task, which will be pauses this task and deletes task progress information.
        /// The task that is canceled cannot be restart.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> value that inducates whether the operation was seccessful.</returns>
        Task<bool> CancelAsync();
    }
}
