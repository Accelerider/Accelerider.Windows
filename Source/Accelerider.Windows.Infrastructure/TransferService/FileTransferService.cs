using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class FileTransferService
    {
        internal const string DefaultManagerName = "default";

        private static readonly ConcurrentDictionary<string, FileDownloaderManager> FileDownloaderManagers = new ConcurrentDictionary<string, FileDownloaderManager>();

        static FileTransferService()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        public static IDownloaderBuilder GetDownloaderBuilder() => new FileDownloaderBuilder();

        public static IDownloaderManager GetDownloaderManager(string name = DefaultManagerName) => FileDownloaderManagers.Get(name);

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
