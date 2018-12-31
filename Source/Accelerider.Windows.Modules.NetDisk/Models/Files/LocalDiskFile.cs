using System;
using System.IO;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    internal class LocalDiskFile : ILocalDiskFile
    {
        [JsonProperty]
        public FileType Type { get; private set; }

        [JsonProperty]
        public FileLocator Path { get; private set; }

        [JsonProperty]
        public long Size { get; private set; }

        [JsonProperty]
        public bool Exists => File.Exists(Path);

        [JsonProperty]
        public DateTime CompletedTime { get; private set; }

        [JsonConstructor]
        private LocalDiskFile() { }

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
