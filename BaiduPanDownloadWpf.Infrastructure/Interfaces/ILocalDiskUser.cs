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
        /// An asynchronous method is used for login operations.
        /// </summary>
        /// <param name="signInInfo">The required information by sign in.</param>
        /// <param name="token">A instance of <see cref="CancellationToken"/>, which is used to cancel the login.</param>
        /// <returns>A instance of <see cref="INetDiskUser"/>, will return null if login failed</returns>
        //Task<INetDiskUser> SignInAsync(SignInInfo signInInfo, CancellationToken token);

        /// <summary>
        /// Gets all net-disk user instances.
        /// </summary>
        /// <returns>A IEnumerable of <see cref="INetDiskUser"/> contains all net-disk user instances.</returns>
        Task<IEnumerable<INetDiskUser>> GetAllNetDiskUsers();

        /// <summary>
        /// Gets a net-disk user instance by id.
        /// </summary>
        /// <param name="userId">The id of user.</param>
        /// <returns>A instance of <see cref="INetDiskUser"/></returns>
        //Task<INetDiskUser> GetNetDiskUserById(Guid userId);

        /// <summary>
        /// Gets all local disk files. (Downloaded records)
        /// </summary>
        /// <returns>A IEnumerable of <see cref="IDiskFile"/> contains all local disk files.</returns>
        IEnumerable<IDiskFile> GetAllLocalDiskFilesAsync();

        /// <summary>
        /// Gets a local disk file by id.
        /// </summary>
        /// <param name="fileId">THe id of file.</param>
        /// <returns>A instance of <see cref="IDiskFile"/></returns>
        //IDiskFile GetLocalDiskFileById(long fileId);
    }
}
