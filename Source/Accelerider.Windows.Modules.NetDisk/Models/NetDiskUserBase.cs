using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models.BaiduCloud;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public abstract class NetDiskUserBase : NetDiskInfo, INetDiskUser, IDisposable
    {
        #region Implements INetDiskUser interface

        public abstract Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

        public abstract Task<IReadOnlyList<IDeletedFile>> GetDeletedFilesAsync();

        public abstract Task<bool> DeleteFileAsync(INetDiskFile file);

        public abstract Task<bool> RestoreFileAsync(IDeletedFile file);

        public abstract IDownloadingFile Download(INetDiskFile from, FileLocator to);

        public abstract IReadOnlyList<IDownloadingFile> GetDownloadingFiles();

        public abstract IReadOnlyList<ILocalDiskFile> GetDownloadedFiles();

        public virtual void ClearDownloadFiles()
        {
            throw new NotImplementedException();
        }

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
