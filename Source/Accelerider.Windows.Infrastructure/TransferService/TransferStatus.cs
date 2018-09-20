namespace Accelerider.Windows.Infrastructure.TransferService
{
    public enum TransferStatus
    {
        /// <summary>
        /// The task is ready. Pending status.
        /// </summary>
        Ready,

        /// <summary>
        /// The task is being downloaded or uploaded. Active status
        /// </summary>
        Transferring,

        /// <summary>
        /// The task is suspended. Pending status.
        /// </summary>
        Suspended,

        /// <summary>
        /// The task failed. Pending status.
        /// </summary>
        Faulted,

        /// <summary>
        /// The task has been completed. End state.
        /// </summary>
        Completed,
    }
}
