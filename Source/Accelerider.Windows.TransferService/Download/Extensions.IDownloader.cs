using System;
using System.Collections.Concurrent;
using System.Linq;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.TransferService
{
    public static class DownloaderExtensions
    {
        private static readonly ConcurrentDictionary<Guid, IManagedTransporterToken> ManagedDownloaderTokens = new ConcurrentDictionary<Guid, IManagedTransporterToken>();

        public static IManagedTransporterToken AsManaged(this IDownloader @this, string managerName = FileTransferService.DefaultManagerName)
        {
            Guards.ThrowIfNull(@this);
            Guards.ThrowIfNullOrEmpty(managerName);

            if (@this.Context == null) throw new InvalidOperationException();

            var id = @this.Id;
            if (!ManagedDownloaderTokens.ContainsKey(id))
            {
                ManagedDownloaderTokens[id] = new ManagedDownloaderTokenImpl(FileTransferService.GetDownloaderManager(managerName), @this);
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

        public static string ToJson(this IDownloader @this)
        {
            if (@this.Status == TransferStatus.Transferring)
                throw new InvalidOperationException();

            return new DownloaderSerializedData
            {
                Tag = @this.Tag,
                Context = @this.Context,
                BlockContexts = @this.BlockContexts.Values.ToList()
            }.ToJson();
        }

        private class ManagedDownloaderTokenImpl : IManagedTransporterToken
        {
            private readonly IDownloaderManager _manager;
            private readonly IDownloader _downloader;

            public ManagedDownloaderTokenImpl(IDownloaderManager manager, IDownloader downloader)
            {
                _manager = manager;
                if (!manager.Add(downloader)) throw new InvalidOperationException();
                _downloader = downloader;
            }

            public void AsNext() => _manager.AsNext(_downloader.Id);

            public void Dispose() => _downloader.Dispose();

            public void Ready() => _manager.Ready(_downloader.Id);

            public void Suspend() => _manager.Suspend(_downloader.Id);
        }
    }
}
