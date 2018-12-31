using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public abstract class NetDiskUserBase : BindableBase, INetDiskUser, IDisposable
    {
        protected const string ArddFileExtension = ".ardd";

        #region Implements INetDiskUser interface

        private string _username;


        public string Id { get; set; }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

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
