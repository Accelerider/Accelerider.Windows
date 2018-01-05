namespace Accelerider.Windows.TransportEngine
{
    /// <summary>
    /// Indicates the status of a file which is in the transfer (download or upload) cycle.
    /// </summary>
    public enum TransportTaskStatus
    {
        /// <summary>
        /// The task is waiting to start. Can be converted to <see cref="Transporting"/>, <see cref="Paused"/>, <see cref="Canceled"/> or <see cref="Faulted"/>.
        /// </summary>
        Waiting,

        /// <summary>
        /// The task is being downloaded or uploaded. 
        /// Can be converted to <see cref="Completed"/>, <see cref="Paused"/>, <see cref="Canceled"/> or <see cref="Faulted"/>.
        /// </summary>
        Transporting,

        /// <summary>
        /// The task is Paused. Can be converted to <see cref="Canceled"/> or <see cref="Waiting"/>.
        /// </summary>
        Paused,

        /// <summary>
        /// The task has been completed. End state.
        /// </summary>
        Completed,

        /// <summary>
        /// The task has been canceled. End state.
        /// </summary>
        Canceled,

        /// <summary>
        /// The task failed. Can be converted to <see cref="Canceled"/> or <see cref="Waiting"/>.
        /// </summary>
        Faulted
    }
}
