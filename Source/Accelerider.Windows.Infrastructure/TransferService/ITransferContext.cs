
using System;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface ITransferContext : ICloneable<ITransferContext>
    {
        /// <summary>
        /// Gets the current status of the task.
        /// </summary>
        TransferStatus Status { get; }

        /// <summary>
        /// Gets the size of the currently completed data.
        /// </summary>
        long CompletedSize { get; }

        /// <summary>
        /// Gets the size of total data.
        /// </summary>
        long TotalSize { get; }

        /// <summary>
        /// Gets the path of local resource, it represents upload source path or download destination path.
        /// </summary>
        string LocalPath { get; }

        /// <summary>
        /// Gets the path of remote resource, it represents upload destination path or download source path.
        /// </summary>
        string RemotePath { get; }

        /// <summary>
        /// Gets or sets the costom data of the user by a key. It will return null if the key does not exist.
        /// </summary>
        /// <param name="key">The key of the resource. </param>
        /// <returns>The costom data. </returns>
        object this[string key] { get; set; }
    }
}
