namespace Accelerider.Windows.Modules.NetDisk.Enumerations
{
    /// <summary>
    /// Indicates th status of a file which is the result of the file is checked after downloaded.
    /// </summary>
    public enum TransportedFileStatus
    {
        /// <summary>
        /// The check result is not available when the file being transferred.
        /// </summary>
        NotAvailable = 0,

        /// <summary>
        /// Check result is normal. For uploading completed files, the results are always displayed.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// The check result contains warnings.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The check result contains errors.
        /// </summary>
        Error = 3
    }
}
