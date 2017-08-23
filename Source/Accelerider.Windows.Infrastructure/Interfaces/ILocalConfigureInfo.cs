using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// The global configuration of the software.
    /// </summary>
    public interface ILocalConfigureInfo
    {
        // For appearance of UI --------------------------------------------------------------------------------------------------
        /// <summary>
        /// The Theme of this software.
        /// </summary>
        string Theme { get; set; } // TODO: Considers creating a class for Theme, which includes BackgoundColor, ForegoundColor, BackgoundPicture and so on. 

        /// <summary>
        /// The language used by the software.
        /// </summary>
        LanguageEnum Language { get; set; }

        // User's information ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the username of Accelerider that needs to be remembered.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the encrypted password of Accelerider user that needs to be remembered.
        /// </summary>
        string PasswordEncrypted { get; set; }

        /// <summary>
        /// Gets or sets a Boolean value that specifies whether automatic login is required.
        /// </summary>
        bool IsAutoSignIn { get; set; }

        // Download settings -----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Specifies whether a dialog box is displayed each time you download.
        /// </summary>
        bool NotDisplayDownloadDialog { get; set; }

        /// <summary>
        /// The storage path of downloaded files.
        /// </summary>
        FileLocation DownloadDirectory { get; set; }

        /// <summary>
        /// The number of download tasks that can be performed at the same time.
        /// </summary>
        int ParallelTaskNumber { get; set; }

        /// <summary>
        /// Maximum download speed.
        /// </summary>
        double SpeedLimit { get; set; }

        // Persistence -----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Persistents data.
        /// </summary>
        void Save();
    }
}
