using System;
using System.IO;
using System.Linq;
using BaiduPanDownloadWpf.Core.Download;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;

namespace BaiduPanDownloadWpf.Core
{
    public class LocalDiskFile : ILocalDiskFile
    {
        public long FileId { get; private set; }
        public FileLocation FilePath { get; private set; }
        public FileTypeEnum FileType { get; private set; }
        public long FileSize { get; private set; }
        public DateTime CompletedTime { get; private set; }

        public static LocalDiskFile GetLocalDiskFile(DownloadInfo info)
        {
            using(var stream=new FileStream(info.DownloadPath,FileMode.Open))
            {
                return new LocalDiskFile()
                {
                    FileId = info.Id,
                    FilePath = new FileLocation(info.DownloadPath),
                    FileType = NetDiskFile.FileTypeDirectory.Select(v => v.Value.Contains(info.DownloadPath.Split('.').Last()) ? v.Key : FileTypeEnum.OtherType).FirstOrDefault(),
                    FileSize =stream.Length,
                    CompletedTime = info.CompletedTime
                };
            }
        }

        
    }
}
