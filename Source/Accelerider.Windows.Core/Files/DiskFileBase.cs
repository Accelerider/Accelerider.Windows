using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files
{
    public abstract class DiskFileBase : IDiskFile
    {
        private static readonly Dictionary<FileTypeEnum, string[]> FileTypeDirectory = new Dictionary<FileTypeEnum, string[]>
        {
            { FileTypeEnum.ApkType, new []{ "apk" }},
            { FileTypeEnum.DocType, new []{ "doc", "docx" } },
            { FileTypeEnum.ExeType, new []{ "exe", "msi", "com", "bat", "sys" } },
            { FileTypeEnum.ImgType, new []{ "png", "jpg", "jpeg", "bmp", "gif", "svg", "ico" } },
            { FileTypeEnum.MusicType, new []{ "mp3", "wav", "aac", "wma", "flac", "ape", "ogg" } },
            { FileTypeEnum.PdfType, new []{ "pdf" } },
            { FileTypeEnum.PptType, new []{ "ppt", "pptx" } },
            { FileTypeEnum.RarType, new []{ "rar", "zip", "7z", "iso", "tar", "zipx", "tgz" } },
            //{ FileTypeEnum.TorrentType, new []{ "torrent" } },
            { FileTypeEnum.TxtType, new []{ "txt", "lrc", "md", "json", "xml", "yml" } },
            { FileTypeEnum.VideoType, new []{ "rmvb", "mp4", "avi", "rm", "wmv", "flv", "f4v", "mkv", "3gp" } },
            { FileTypeEnum.XlsType, new []{ "xls", "xlsx" } },
        };

        public FileTypeEnum FileType => Directory.Exists(FilePath.FullPath)
            ? FileTypeEnum.FolderType
            : (from item in FileTypeDirectory
                where item.Value.Contains(FilePath.FileExtension)
                select item.Key).SingleOrDefault();

        public abstract Task<bool> DeleteAsync();

        public FileLocation FilePath { get; set; }
        public DataSize FileSize { get; set; }
    }
}
