using System;
using System.IO;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    internal class LocalDiskFile : ILocalDiskFile
    {
        public FileType Type { get; }

        public FileLocator Path { get; }

        public long Size { get; }

        public bool Exists => File.Exists(Path);

        public DateTime CompletedTime { get; }

        private LocalDiskFile(IDownloadingFile downloadingFile)
        {
            Type = downloadingFile.File.Type;
            Path = downloadingFile.DownloadInfo.Context.LocalPath;
            Size = downloadingFile.DownloadInfo.Context.TotalSize;
            CompletedTime = DateTime.Now;
        }

        public static ILocalDiskFile Create(IDownloadingFile file)
        {
            return new LocalDiskFile(file);
        }
    }
}
