using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelerider.Windows.Modules.NetDisk.Enumerations;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.Infrastructure
{
    public static class FileLocatorExtensions
    {
        public static readonly Dictionary<FileType, string[]> FileTypeMapping = new Dictionary<FileType, string[]>
        {
            { FileType.ApkType, new []{ "apk" }},
            { FileType.DocType, new []{ "doc", "docx" } },
            { FileType.ExeType, new []{ "exe" } },
            { FileType.ImgType, new []{ "png", "jpg", "jpeg", "bmp", "git" } },
            { FileType.MixFileType, new []{ "mix" } },
            { FileType.MusicType, new []{ "mp3", "wav", "aac", "wma", "flac", "ape", "ogg" } },
            { FileType.PdfType, new []{ "pdf" } },
            { FileType.PptType, new []{ "ppt", "pptx" } },
            { FileType.RarType, new []{ "rar", "zip", "7z", "iso" } },
            { FileType.TorrentType, new []{ "torrent" } },
            { FileType.TxtType, new []{ "txt", "lrc" } },
            { FileType.VideoType, new []{ "rmvb", "mp4", "avi", "rm", "wmv", "flv", "f4v", "mkv", "3gp" } },
            { FileType.XlsType, new []{ "xls", "xlsx" } },
        };

        public static FileType GetFileType(this FileLocator fileLocator)
        {
            if (fileLocator == null || string.IsNullOrWhiteSpace(fileLocator.FileName)) return default;

            var fileExtension = Path.GetExtension(fileLocator.FileName).Substring(1);
            return (from item in FileTypeMapping where item.Value.Contains(fileExtension) select item.Key).FirstOrDefault();
        }
    }
}