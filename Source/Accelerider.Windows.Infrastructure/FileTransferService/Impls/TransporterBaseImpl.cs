using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal abstract class TransporterBaseImpl : ITransporter
    {
        private TransferStatus _status;

        public event EventHandler<TransferStatusChangedEventArgs> StatusChanged;

        public TransporterToken Token { get; } = new TransporterToken();

        public TransferStatus Status
        {
            get => _status;
            protected set
            {
                if (_status == value) return;
                OnStatusChanged(_status, _status = value);
            }
        }

        public virtual DataSize CompletedSize { get; protected set; }

        public DataSize TotalSize { get; protected set; }

        public FileLocator LocalPath { get; protected set; }

        public abstract void Start();

        public abstract void Suspend();

        #region Implements IDisposable interface

        protected bool _disposed;

        ~TransporterBaseImpl() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        #endregion

        protected virtual void OnStatusChanged(TransferStatus oldStatus, TransferStatus newStatus) =>
            StatusChanged?.Invoke(this, new TransferStatusChangedEventArgs(oldStatus, newStatus));
    }
}
