using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class FileTransferTask : ObservableBase<BlockTransferContext>, IFileTransferTask
    {
        private TransferStatus _status;
        private TransferContext _context;

        public TransferStatus Status
        {
            get => _status;
            private set => SetProperty(ref _status, value);
        }

        public TransferContext Context
        {
            get => _context;
            internal set => SetProperty(ref _context, value);
        }

        public IDisposable Subscribe(IObserver<BlockTransferContext> observer)
        {
            throw new NotImplementedException();
        }

        protected override IDisposable SubscribeCore(IObserver<BlockTransferContext> observer)
        {
            throw new NotImplementedException();
        }

        public void Activate()
        {

            Status = TransferStatus.Transferring;
        }

        public void Suspend()
        {

            Status = TransferStatus.Suspended;
        }

        public void Dispose()
        {
            Status = TransferStatus.Disposed;
        }

        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
