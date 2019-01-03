namespace Accelerider.Windows.Modules.NetDisk.Enumerations
{
    /// <summary>
    /// An enum is used to mark the file type.
    /// </summary>
    public enum FileType
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
}
