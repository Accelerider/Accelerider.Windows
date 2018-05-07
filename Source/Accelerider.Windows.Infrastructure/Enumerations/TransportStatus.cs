namespace Accelerider.Windows.Infrastructure
{
    /// <summary>
    /// Indicates the status of a file which is in the transfer (download or upload) cycle.
    /// </summary>
    public enum TransportStatus
    {
        /// <summary>
        /// The task is ready. Can be converted to <see cref="Transporting"/> or <see cref="Suspended"/>. Start state.
        /// </summary>
        Ready,

        /// <summary>
        /// The task is being downloaded or uploaded. 
        /// Can be converted to <see cref="Completed"/>, <see cref="Suspended"/> or <see cref="Faulted"/>.
        /// </summary>
        Transporting,

        /// <summary>
        /// The task is suspended. Can be converted to <see cref="Ready"/> or <see cref="Faulted"/>.
        /// </summary>
        Suspended,

        /// <summary>
        /// The task failed. Can be converted to <see cref="Ready"/>. Start state.
        /// </summary>
        Faulted,

        /// <summary>
        /// The task has been completed. Start/End state.
        /// </summary>
        Completed
    }
}
