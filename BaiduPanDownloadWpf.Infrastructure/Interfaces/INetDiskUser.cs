using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// An aggregate object represents net-disk user.
    /// </summary>
    public interface INetDiskUser
    {
        /// <summary>
        /// Gets the local path of the user's head image file.
        /// </summary>
        Uri HeadImageUri { get; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the nick name of the user.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// Gets the total size(byte) of net-disk.
        /// </summary>
        long TotalSpace { get; }

        /// <summary>
        /// Gets the free size(byte) of net-disk.
        /// </summary>
        long FreeSpace { get; }

        /// <summary>
        /// Gets the used size(byte) of net-disk.
        /// </summary>
        long UsedSpace { get; }

        /// <summary>
        /// Gets the root file, which represents root directory of the net-disk.
        /// </summary>
        INetDiskFile RootFile { get; }


        /// <summary>
        /// Gets the uncompleted download tasks.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDiskFile> GetUncompletedFiles();

        /// <summary>
        /// Gets the completed download tasks.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILocalDiskFile> GetCompletedFiles();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Uri> ShareFilesAsync(IEnumerable<INetDiskFile> files, string password = null);

        /// <summary>
        /// Gets the list of the shared files.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();

        /// <summary>
        /// Gets the list of the deleted files.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();

        /// <summary>
        /// Updates the information of the user.
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync();
    }
}
