using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// An aggregate object represents local disk user.
    /// </summary>
    public interface ILocalDiskUser
    {
        /// <summary>
        /// Gets or sets the current net-disk user.
        /// </summary>
        INetDiskUser CurrentNetDiskUser { get; set; }

        /// <summary>
        /// The download path of resources.
        /// </summary>
        string DownloadDirectory { get; set; }

        /// <summary>
        /// The maximum number of parallel download tasks.
        /// </summary>
        int ParallelTaskNumber { get; set; }

        /// <summary>
        /// The maximum number of download threads.
        /// </summary>
        int DownloadThreadNumber { get; set; }

        /// <summary>
        /// Gets a boolean value to indicate whether the password is remembered.
        /// </summary>
        bool IsRememberPassword { get; set; }

        /// <summary>
        /// Gets a boolean value to indicate whether sign in account automatically.
        /// </summary>
        bool IsAutoSignIn { get; set; }

        /// <summary>
        /// Gets all net-disk user instances.
        /// </summary>
        /// <returns>A IEnumerable of <see cref="INetDiskUser"/> contains all net-disk user instances.</returns>
        Task<IEnumerable<INetDiskUser>> GetAllNetDiskUsers();
    }
}
