using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class FileTransferService
    {
        internal const string DefaultManagerName = "default";

        private static readonly ConcurrentDictionary<string, FileDownloaderManager> FileDownloaderManagers = new ConcurrentDictionary<string, FileDownloaderManager>();
        private static readonly ConcurrentDictionary<string, FileUploaderManager> FileUploaderManagers = new ConcurrentDictionary<string, FileUploaderManager>();

        static FileTransferService()
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
        }

        public static IDownloaderBuilder GetDownloaderBuilder() => new FileDownloaderBuilder();

        public static ITransporterManager<IDownloader> GetDownloaderManager(string name = DefaultManagerName) => FileDownloaderManagers.Get(name);

        public static IUploaderBuilder GetUploaderBuilder() => new FileUploaderBuilder();

        public static ITransporterManager<IUploader> GetUploaderManager(string name = DefaultManagerName) => FileUploaderManagers.Get(name);

        internal static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key) where TValue : new()
        {
            if (!@this.ContainsKey(key))
            {
                @this[key] = new TValue();
            }

            return @this[key];
        }
    }
}
