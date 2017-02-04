using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces.Files
{
    /// <summary>
    /// A entity represents net-disk file.
    /// </summary>
    public interface INetDiskFile : IDiskFile
    {
        /// <summary>
        /// Gets the time at which the file was created.
        /// </summary>
        DateTime CreatedTime { get; }

        /// <summary>
        /// Gets the time at which the file was motified.
        /// </summary>
        DateTime MotifiedTime { get; }

        /// <summary>
        /// Gets childern file of current flie by asynchronous.
        /// If current file <see cref="IDiskFile.FileType"/> is <see cref="FileTypeEnum.FolderType"/>, return a instance of <see cref="IEnumerable{INetDiskFile}"/> type represents the childern file,
        /// otherwise, return null. 
        /// </summary>
        /// <returns>The children file of current file. </returns>
        Task<IEnumerable<INetDiskFile>> GetChildrenAsync();

        Task DownloadAsync();

        Task<bool> DeleteAsync();

        Task<bool> RestoreAsync();
    }
}
