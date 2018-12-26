using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferService.WpfInteractions;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public abstract class NetDiskUserBase : INetDiskUser
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public Uri Avatar { get; protected set; }

        public (long Used, long Total) Capacity { get; protected set; }

        public DisplayDataSize UsedCapacity { get; set; }

        [JsonIgnore]
        protected List<TransferItem> DownloadItems { get; private set; }

        [JsonProperty]
        protected List<string> DownloadingFilePaths { get; private set; } = new List<string>();


        public abstract Task<bool> RefreshAsync();

        public abstract Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

        public Task<IReadOnlyList<IDeletedFile>> GetDeletedFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFileAsync(INetDiskFile file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestoreFileAsync(IDeletedFile file)
        {
            throw new NotImplementedException();
        }

        public abstract IDownloadingFile Download(INetDiskFile from, FileLocator to);

        public virtual IReadOnlyList<IDownloadingFile> GetDownloadingFiles()
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyList<ILocalDiskFile> GetDownloadedFiles()
        {
            throw new NotImplementedException();
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
            if (DownloadItems == null) DownloadItems = new List<TransferItem>();

            GetDownloadItemsFromLocalDisk().Where(item => !DownloadItems.Contains(item)).ForEach(item => DownloadItems.Add(item));

            return DownloadItems;
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
