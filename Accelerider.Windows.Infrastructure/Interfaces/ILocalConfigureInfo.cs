using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// The global configuration of the software.
    /// </summary>
    public interface ILocalConfigureInfo
    {
        /// <summary>
        /// The Theme of this software.
        /// </summary>
        string Theme { get; set; } // TODO: Considers creating a class for Theme, which includes BackgoundColor, ForegoundColor, BackgoundPicture and so on. 

        /// <summary>
        /// The language used by the software.
        /// </summary>
        LanguageEnum Language { get; set; }

        /// <summary>
        /// Specifies whether a dialog box is displayed each time you download.
        /// </summary>
        bool IsDisplayDownloadDialog { get; set; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> value to contain the encrypted password of all users that is remembered.
        /// </summary>
        List<string> PasswordEncrypteds { get; }

        /// <summary>
        /// Gets a <see cref="int"/> value to indicate the index of the account that needs to login automatically.
        /// </summary>
        int AutoLoginAccountIndex { get; set; }

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
