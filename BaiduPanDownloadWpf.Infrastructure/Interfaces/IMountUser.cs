using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// An aggregate object represents local disk user.
    /// </summary>
    public interface IMountUser
    {
        /// <summary>
        /// Gets the name of user.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the encrypted password.
        /// </summary>
        string PasswordEncrypted { get; }

        /// <summary>
        /// Gets or sets the current net-disk user.
        /// </summary>

        /// <summary>
        /// Gets a boolean value to indicate whether the password is remembered.
        /// </summary>
        bool IsRememberPassword { get; set; }

        /// <summary>
        /// Gets a boolean value to indicate whether sign in account automatically.
        /// </summary>
        bool IsAutoSignIn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsConnectedServer { get; }

        /// <summary>
        /// Gets all net-disk user instances.
        /// </summary>
        /// <returns>A IEnumerable of <see cref="INetDiskUser"/> contains all net-disk user instances.</returns>
        IEnumerable<INetDiskUser> GetAllNetDiskUsers();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        void SetAccountInfo(string username, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task SignInAsync();

        /// <summary>
        /// 
        /// </summary>
        void SignOut();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        INetDiskUser GetCurrentNetDiskUser();

        // TODO: Temporary solution.
        void PasueDownloadTask(long fileId);
        void RestartDownloadTask(long fileId);
        void CancelDownloadTask(long fileId);
    }
}
