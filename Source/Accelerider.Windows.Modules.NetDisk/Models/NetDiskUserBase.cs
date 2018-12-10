using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferService.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models.SixCloud;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public abstract class NetDiskUserBase : INetDiskUser
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public Uri Avatar { get; protected set; }

        public DisplayDataSize TotalCapacity { get; set; }

        public DisplayDataSize UsedCapacity { get; set; }

        [JsonIgnore]
        protected List<TransferItem> DownloadItems { get; private set; }

        [JsonProperty]
        protected List<string> DownloadingFilePaths { get; private set; } = new List<string>();


        public abstract Task RefreshUserInfoAsync();

        public abstract Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

        public abstract TransferItem Download(ILazyTreeNode<INetDiskFile> from, FileLocator to);

        public abstract Task UploadAsync(FileLocator from, INetDiskFile to, Action<TransferItem> callback);

        public IReadOnlyCollection<TransferItem> GetDownloadItems()
        {
            return GetDownloadItemsInternal().AsReadOnly();
        }

        protected void SaveDownloadItem(TransferItem downloadItem)
        {
            var downloadItems = GetDownloadItemsInternal();
            downloadItems.Add(downloadItem);
        }

        protected abstract IDownloaderBuilder ConfigureDownloaderBuilder(IDownloaderBuilder builder);

        protected abstract IRemotePathProvider GetRemotePathProvider(string jsonText);

        private List<TransferItem> GetDownloadItemsInternal()
        {
            return DownloadItems ?? (DownloadItems = GetDownloadItemsFromLocalDisk().ToList());
        }

        private IEnumerable<TransferItem> GetDownloadItemsFromLocalDisk()
        {
            return DownloadingFilePaths
                .Where(File.Exists)
                .Select(File.ReadAllText)
                .Select(item => ConfigureDownloaderBuilder(FileTransferService.GetDownloaderBuilder())
                    .Build(item, GetRemotePathProvider))
                .Select(item => new TransferItem(item));
        }
    }
}
