using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        private static readonly ConcurrentDictionary<long, ILocalDiskFile> _cache = new ConcurrentDictionary<long, ILocalDiskFile>();

        public static ILocalDiskFile Create(IDownloadingFile file)
        {
            var hash = file.GetHashCode();

            if (_cache.TryGetValue(hash, out ILocalDiskFile result))
            {
                return result;
            }
              
            result = new LocalDiskFile(file);
            if (_cache.TryAdd(hash, result) && _cache.Count > 50)
            {
                var firstKey = _cache.FirstOrDefault().Key;
                _cache.TryRemove(firstKey, out _);
            }

            return result;
        }
    }
}
