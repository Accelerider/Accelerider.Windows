using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal sealed class TransporterRegistryImpl : ITransporterRegistry
    {
        private TransporterBaseImpl _transporter;
        private TransferContext _context;

        private Action _disposeCallback;

        private IManagedTransporterToken _managedTransporterToken;
        private IUnmanagedTransporterToken _unmanagedTransporterToken;


        public TransporterRegistryImpl(TransporterBaseImpl transporter, TransferContext context, Action disposeCallback)
        {
            _transporter = transporter;
            _context = context;

            _disposeCallback = disposeCallback;
        }

        public IManagedTransporterToken AsManaged() =>
            _managedTransporterToken ?? (_managedTransporterToken = new ManagedTransporterTokenImpl(_transporter, _context, Dispose));

        public IUnmanagedTransporterToken AsUnmanaged() =>
            _unmanagedTransporterToken ?? (_unmanagedTransporterToken = new UnmanagedTransporterTokenImpl(_transporter, Dispose));

        private void Dispose()
        {
            _disposeCallback?.Invoke();
            _disposeCallback = null;
            _transporter = null;
            _context = null;
            _managedTransporterToken = null;
            _unmanagedTransporterToken = null;
        }
    }

    internal sealed class UnmanagedTransporterTokenImpl : TransporterTokenBaseImpl, IUnmanagedTransporterToken
    {
        public UnmanagedTransporterTokenImpl(TransporterBaseImpl transpoter, Action disposeCallback)
            : base(transpoter, disposeCallback)
        {
        }

        public void Start() => _transpoter.Start();

        public void Suspend() => _transpoter.Suspend();
    }

    internal sealed class ManagedTransporterTokenImpl : TransporterTokenBaseImpl, IManagedTransporterToken
    {
        private TransferContext _context;

        public ManagedTransporterTokenImpl(TransporterBaseImpl transpoter, TransferContext context, Action disposeCallback)
            : base(transpoter, disposeCallback)
        {
            _context = context;
        }

        public void Ready() => _transpoter.Status = TransferStatus.Ready;

        public void Suspend() => _transpoter.Suspend();

        public void AsNext() => _context.AsNext(_transpoter);

        public override void Dispose()
        {
            base.Dispose();
            _context = null;
        }
    }

    internal class TransporterTokenBaseImpl : IDisposable
    {
        protected TransporterBaseImpl _transpoter;
        private Action _disposeCallback;

        public TransporterTokenBaseImpl(TransporterBaseImpl transpoter, Action disposeCallback)
        {
            _transpoter = transpoter;
            _disposeCallback = disposeCallback;
            _transpoter.StatusChanged += OnStatusChanged;
        }

        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {
            if (e.NewStatus != TransferStatus.Disposed) return;

            _transpoter = null;
            Dispose();
        }

        public virtual void Dispose()
        {
            _disposeCallback?.Invoke();
            _disposeCallback = null;

            if (_transpoter == null) return;

            _transpoter.StatusChanged -= OnStatusChanged;
            _transpoter.Dispose();
            _transpoter = null;
        }
    }
}
