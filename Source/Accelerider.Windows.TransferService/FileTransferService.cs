using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace Accelerider.Windows.TransferService
{
    public static class FileTransferService
    {
        public const string DefaultManagerName = "default";

        private static readonly ConcurrentDictionary<string, FileDownloaderManager> FileDownloaderManagers = new ConcurrentDictionary<string, FileDownloaderManager>();

        static FileTransferService()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        public static IDownloaderBuilder GetDownloaderBuilder() => new FileDownloaderBuilder();

        public static IDownloaderManager GetDownloaderManager(string name = DefaultManagerName) => FileDownloaderManagers.Get(name);
    }
}
