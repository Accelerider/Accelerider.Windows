namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    /*
     * * UI layer operations: (Operates the transporter queue)
     * [Restart()]:           |               | Suspended | Faulted |           --> Ready*
     * [Cancel()]:  Ready     | Transferring* | Suspended | Faulted | Completed --> Disposed
     * [AsNext()]:  Any Ready |               |           |         |           --> Top Ready
     * 
     * * Transporter operations: (Operates itself)
     * [Start()]:   Ready*    |               | Suspended | Faulted |           --> Transferring*
     * [Dispose()]: Ready     |               | Suspended | Faulted | Completed --> Disposed
     *
     * * Common operations:
     * [Suspend()]: Ready     | Transferring  |           |         |           --> Suspended
     */

    /*
     *              | Ready | Transferring | Suspended | Faulted | Completed | Disposed |
     * Ready        |       | ■■■■■■■■■■■■ |           | ■■■■■■■ |           | ■■■■■■■■ |
     * Transferring |       |              |           |         |           |          |
     * Suspended    |       | ■■■■■■■■■■■■ |           |         |           |          |
     * Faulted      |       | ■■■■■■■■■■■■ |           |         |           | ■■■■■■■■ |
     * Completed    |       |              |           |         |           |          |
     * Disposed     |       |              |           |         |           |          |
     */

    /// <summary>
    /// Indicates the status of a file which is in the transfer (download or upload) cycle.
    /// </summary>
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

        /// <summary>
        /// The task has been disposed. End state.
        /// </summary>
        Disposed
    }
}
