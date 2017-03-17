using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// The global configuration of the software.
    /// </summary>
    public interface ILocalConfigInfo
    {
        /// <summary>
        /// The Theme of this software.
        /// </summary>
        string Theme { get; set; }

        /// <summary>
        /// the background of this software.
        /// </summary>
        string Background { get; set; }

        /// <summary>
        /// The language used by the software.
        /// </summary>
        LanguageEnum Language { get; set; }

        /// <summary>
        /// Specifies whether a dialog box is displayed each time you download.
        /// </summary>
        bool IsDisplayDownloadDialog { get; set; }

        /// <summary>
        /// The storage path of downloaded files.
        /// </summary>
        string DownloadDirectory { get; set; }

        /// <summary>
        /// The number of download tasks that can be performed at the same time.
        /// </summary>
        int ParallelTaskNumber { get; set; }

        /// <summary>
        /// Maximum download speed.
        /// </summary>
        double SpeedLimit { get; set; }

        /// <summary>
        /// Persistents data.
        /// </summary>
        void Save();
    }
}
