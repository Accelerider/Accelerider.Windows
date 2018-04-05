using System;

namespace Accelerider.Windows.Infrastructure
{
    /// <summary>
    /// An enum is used to mark the file type.
    /// </summary>
    public enum FileTypeEnum
    {
        /// <summary>
        /// The file format is unknown.
        /// </summary>
        OtherType,

        /// <summary>
        /// Indicates that the file is a floder.
        /// </summary>
        FolderType,

        /// <summary>
        /// Indicates that the file extension is apk.
        /// </summary>
        ApkType,

        /// <summary>
        /// Indicates that the file extension is pdf. 
        /// </summary>
        PdfType,

        /// <summary>
        /// Indicates that the file is a text file (e.g.: txt, lrc).
        /// </summary>
        TxtType,

        /// <summary>
        /// Indicates that the file is a audio file (e.g.: mp3, wav, aac, wma, flac, ape, ogg)
        /// </summary>
        MusicType,

        /// <summary>
        /// Indicates that the file is a image (e.g.: png, jpg, jpeg, bmp, git).
        /// </summary>
        ImgType,

        /// <summary>
        /// Indicates that the file is a video file (e.g.: rmvb, mp4, avi, rm, wmv, flv, f4v, mkv, 3gp)
        /// </summary>
        VideoType,

        /// <summary>
        /// Indicates that the file extension is doc or docx.
        /// </summary>
        DocType,

        /// <summary>
        /// Indicates that the file extension is xls or xlsx.
        /// </summary>
        XlsType,

        /// <summary>
        /// Indicates that the file extension is ppt or pptx.
        /// </summary>
        PptType,

        /// <summary>
        /// Indicates that the file extension is exe.
        /// </summary>
        ExeType,

        /// <summary>
        /// Indicates that the file is in a compressed format (e.g.: rar, zip, 7z, iso).
        /// </summary>
        RarType,

        /// <summary>
        /// Indicates that the file extension is torrent.
        /// </summary>
        TorrentType,

        /// <summary>
        /// Indicates that the file extension is mix.
        /// </summary>
        MixFileType,
    }

    /// <summary>
    /// Represents the unit of data size.
    /// </summary>
    public enum SizeUnitEnum
    {
        /// <summary>
        /// Byte.
        /// </summary>
        B,

        /// <summary>
        /// K byte.
        /// </summary>
        K,

        /// <summary>
        /// M byte.
        /// </summary>
        M,

        /// <summary>
        /// G byte.
        /// </summary>
        G,

        /// <summary>
        /// T byte.
        /// </summary>
        T,

        /// <summary>
        /// P byte.
        /// </summary>
        P,
    }

    /// <summary>
    /// Represents the result of the sharing.
    /// </summary>
    public enum ShareStateCode
    {
        /// <summary>
        /// The file was shared normally.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The file sharing failed.
        /// </summary>
        Failure = 1,

        /// <summary>
        /// Shares the file failed because the file was not approved.
        /// </summary>
        NotPassed = 4,

        /// <summary>
        /// Shares the file failed because the file has been deleted.
        /// </summary>
        Deleted = 9
    }

    /// <summary>
    /// Indicates the status of a file which is in the transfer (download or upload) cycle.
    /// </summary>
    public enum TransferTaskStatusEnum
    {
        /// <summary>
        /// The task created, Can only be converted to <see cref="Waiting"/>.
        /// </summary>
        Created,

        /// <summary>
        /// The task is waiting to start. Can be converted to <see cref="Transferring"/>, <see cref="Paused"/>, <see cref="Canceled"/> or <see cref="Faulted"/>.
        /// </summary>
        Waiting,

        /// <summary>
        /// The task is being downloaded or uploaded. 
        /// Can be converted to <see cref="Completed"/>, <see cref="Paused"/>, <see cref="Canceled"/> or <see cref="Faulted"/>.
        /// </summary>
        Transferring,

        /// <summary>
        /// The task is Paused. Can be converted to <see cref="Canceled"/> or <see cref="Waiting"/>.
        /// </summary>
        Paused,

        /// <summary>
        /// The task has been completed. End state.
        /// </summary>
        Completed,

        /// <summary>
        /// The task has been canceled. End state.
        /// </summary>
        Canceled,

        /// <summary>
        /// The task failed. Can be converted to <see cref="Canceled"/> or <see cref="Waiting"/>.
        /// </summary>
        Faulted
    }

    /// <summary>
    /// Indicates th status of a file which is the result of the file is checked after downloaded.
    /// </summary>
    public enum FileCheckStatusEnum
    {
        /// <summary>
        /// The check result is not available when the file being transferred.
        /// </summary>
        NotAvailable = 0,

        /// <summary>
        /// Check result is normal. For uploading completed files, the results are always displayed.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// The check result contains warnings.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The check result contains errors.
        /// </summary>
        Error = 3
    }

    public enum LanguageEnum
    {
        Chinese,

        English,

        Japanese,
    }
}
