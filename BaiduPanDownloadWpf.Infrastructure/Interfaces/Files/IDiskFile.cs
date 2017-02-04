namespace BaiduPanDownloadWpf.Infrastructure.Interfaces.Files
{
    /// <summary>
    /// A entity contains the basic information of the file, which can represents a local disk file or provites information for the file in download.
    /// </summary>
    public interface IDiskFile : IFile
    {
        /// <summary>
        /// Gets a instance of <see cref="FileLocation"/> which represents the file location information.
        /// </summary>
        FileLocation FilePath { get; }

        /// <summary>
        /// Gets the size of the file based on byte. If the <see cref="FileType"/> is <see cref="FileTypeEnum.FolderType"/>, return 0.
        /// </summary>
        long FileSize { get; }
    }
}
