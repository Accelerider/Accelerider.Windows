using System.Collections.Concurrent;
using System.Net;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static partial class FileTransferService
    {
        private const string DefaultManagerName = "default";

        private static readonly ConcurrentDictionary<string, FileDownloaderManager> Managers = new ConcurrentDictionary<string, FileDownloaderManager>();

        static FileTransferService()
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
        }

        public static IDownloaderBuilder GetFileDownloaderBuilder() => new FileDownloaderBuilder();

        public static IDownloaderManager GetFileDownloaderManager(string name = DefaultManagerName)
        {
            if (!Managers.ContainsKey(name))
            {
                Managers[name] = new FileDownloaderManager();
            }

            return Managers[name];
        }
    }
}
