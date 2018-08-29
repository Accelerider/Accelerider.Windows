namespace Accelerider.Windows.Infrastructure.Enumerations
{
    /// <summary>
    /// Represents the result of the sharing.
    /// </summary>
    public enum ShareStatus
    {
        /// <summary>
        /// The file was shared normally.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The file sharing failed.
        /// </summary>
        Failure = 1,

        /// <summary>
        /// Shares the file failed because the file was not approved.
        /// </summary>
        NotPassed = 4,

        /// <summary>
        /// Shares the file failed because the file has been deleted.
        /// </summary>
        Deleted = 9
    }

}
