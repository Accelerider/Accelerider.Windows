using System;
using System.IO;
using System.Linq;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using BaiduPanDownloadWpf.Core.Download.DwonloadCore;

namespace BaiduPanDownloadWpf.Core
{
    public class LocalDiskFile : ILocalDiskFile
    {
        public long FileId { get; private set; }
        public FileLocation FilePath { get; private set; }
        public FileTypeEnum FileType { get; private set; }
        public long FileSize { get; private set; }
        public DateTime CompletedTime { get; private set; }


        public LocalDiskFile(long id, string path, FileTypeEnum type, long size, DateTime time)
        {
            FileId = id;
            FilePath = new FileLocation(path);
            FileType = type;
            FileSize = size;
            CompletedTime = time;
            
        }
    }
}
