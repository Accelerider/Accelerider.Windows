using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IManagedDownloaderToken
    {
        void Ready();

        void AsNext();
    }

    public static partial class FileTransferService
    {
        private static readonly ConcurrentDictionary<Guid, IManagedDownloaderToken> ManagedDownloaderTokens = new ConcurrentDictionary<Guid, IManagedDownloaderToken>();

        public static IManagedDownloaderToken AsManaged(this IDownloader @this, string managerName = DefaultManagerName)
        {
            Guards.ThrowIfNull(@this);
            Guards.ThrowIfNullOrEmpty(managerName);

            if (@this.Context == null) throw new InvalidOperationException();

            var id = @this.Context.Id;
            if (!ManagedDownloaderTokens.ContainsKey(id))
            {
                ManagedDownloaderTokens[id] = new ManagedDownloaderTokenImpl(GetFileDownloaderManager(managerName), @this);
            }

            return ManagedDownloaderTokens[id];
        }

        public static long GetCompletedSize(this IDownloader @this)
        {
            Guards.ThrowIfNull(@this);

            return @this.BlockContexts?.Values.Sum(item => item.CompletedSize) ?? 0;
        }

        public static long GetTotalSize(this IDownloader @this)
        {
            Guards.ThrowIfNull(@this);

            return @this.Context?.TotalSize ?? 0;
        }


        private class ManagedDownloaderTokenImpl : IManagedDownloaderToken
        {
            private readonly FileDownloaderManager _manager;
            private readonly Guid _id;

            public ManagedDownloaderTokenImpl(FileDownloaderManager manager, IDownloader downloader)
            {
                _manager = manager;
                if (!manager.Add(downloader)) throw new InvalidOperationException();
                _id = downloader.Context.Id;
            }

            public void AsNext() => _manager.AsNext(_id);

            public void Ready() => _manager.Ready(_id);
        }
    }
}
