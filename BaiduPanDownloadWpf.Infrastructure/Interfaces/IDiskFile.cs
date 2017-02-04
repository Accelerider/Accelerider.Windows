using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// A entity contains the basic information of the file, which can represents a local disk file or provites information for the file in download.
    /// </summary>
    public interface IDiskFile
    {
        /// <summary>
        /// Gets the ID of the file.
        /// </summary>
        long FileId { get; }

        /// <summary>
        /// Gets a instance of <see cref="FileLocation"/> which represents the file location information.
        /// </summary>
        FileLocation FilePath { get; }

        /// <summary>
        /// Gets the type of the file.
        /// </summary>
        FileTypeEnum FileType { get; }

        /// <summary>
        /// Gets the size of the file. If the <see cref="FileType"/> is <see cref="FileTypeEnum.FolderType"/>, return null.
        /// </summary>
        long FileSize { get; }
    }
}
