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
        string HeadImagePath { get; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets the nick name of the user.
        /// </summary>
        string NickName { get; }

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
        IEnumerable<ILocalDiskFile> GetUncompletedFiles();

        /// <summary>
        /// Gets the completed download tasks.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILocalDiskFile> GetCompletedFiles();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<ISharedFile>> GetSharedFilesAsync();

        /// <summary>
        /// Gets the list of the deleted files.
        /// </summary>
        /// <returns></returns>
        Task<IList<IDeletedFile>> GetDeletedFilesAsync();
    }
}
