using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models.BaiduCloud;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public abstract class NetDiskUserBase : BindableBase, INetDiskUser, IDisposable
    {
        protected const string ArddFileExtension = ".ardd";

        #region Implements INetDiskUser interface

        private string _username;
        private Uri _avatar;
        private (long Used, long Total) _capacity;

        public string Id { get; protected set; }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public Uri Avatar
        {
            get => _avatar;
            protected set => SetProperty(ref _avatar, value);
        }

        public (long Used, long Total) Capacity
        {
            get => _capacity;
            protected set => SetProperty(ref _capacity, value);
        }

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
