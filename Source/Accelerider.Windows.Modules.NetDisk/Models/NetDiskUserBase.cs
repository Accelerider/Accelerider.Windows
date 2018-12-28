using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public abstract class NetDiskUserBase : INetDiskUser, IDisposable
    {
        protected const string ArddFileExtension = ".ardd";

        #region Implements INetDiskUser interface

        public string Id { get; set; }

        public string Username { get; set; }

        public Uri Avatar { get; protected set; }

        public (long Used, long Total) Capacity { get; protected set; }

        public abstract Task<bool> RefreshAsync();

        public abstract Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

        public abstract Task<IReadOnlyList<IDeletedFile>> GetDeletedFilesAsync();

        public abstract Task<bool> DeleteFileAsync(INetDiskFile file);

        public abstract Task<bool> RestoreFileAsync(IDeletedFile file);

        public abstract IDownloadingFile Download(INetDiskFile from, FileLocator to);

        public abstract IReadOnlyList<IDownloadingFile> GetDownloadingFiles();

        public abstract IReadOnlyList<ILocalDiskFile> GetDownloadedFiles();

        #endregion

        #region Implements IDisposable interface

        protected virtual void Dispose(bool disposing)
        {
            // TODO: Suspend all downloading task, and persist them.

            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
