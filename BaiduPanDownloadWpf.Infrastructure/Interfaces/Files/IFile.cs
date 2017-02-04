using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces.Files
{
    public interface IFile
    {
        /// <summary>
        /// Gets the ID of the file.
        /// </summary>
        long FileId { get; }

        /// <summary>
        /// Gets the type of the file.
        /// </summary>
        FileTypeEnum FileType { get; }
    }
}
