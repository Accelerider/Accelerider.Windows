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
        Notpassed = 4,

        /// <summary>
        /// Shares the file failed because the file has been deleted.
        /// </summary>
        Deleted = 9
    }

    /// <summary>
    /// Indicates the status of a file which is in th e transfer (download or upload) cycle.
    /// </summary>
    [Flags]
    public enum TransferStateEnum
    {
        /// <summary>
        /// The task created, waiting to start. Can be converted to <see cref="Transfering"/>, <see cref="Paused"/>, <see cref="Canceled"/> or <see cref="Faulted"/>.
        /// </summary>
        Waiting = 0,

        /// <summary>
        /// The task is being downloaded. Can be converted to <see cref="Paused"/>, <see cref="Completed"/>, <see cref="Canceled"/> or <see cref="Faulted"/>.
        /// </summary>
        Transfering = 1,

        /// <summary>
        /// The task is Paused. Can be converted to <see cref="Canceled"/> or <see cref="Waiting"/>.
        /// </summary>
        Paused = 2,

        /// <summary>
        /// The task has been completed. End state.
        /// </summary>
        Completed = 4,

        /// <summary>
        /// The task has been canceled. End state.
        /// </summary>
        Canceled = 8,

        /// <summary>
        /// The task failed. End state.
        /// </summary>
        Faulted = 16
    }

    public enum LanguageEnum
    {

    }
}
