using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal sealed class TransporterRegistry : ITransporterRegistry
    {
        private TransferContext _context;
        private TransporterBase _transporter;

        public TransporterRegistry(TransporterBase transporter, TransferContext context)
        {
            _transporter = transporter;
            _context = context;
        }

        public IManagedTransporterToken AsManaged() =>
            GetTransporterToken(() => new ManagedTransporterToken(_transporter, _context));

        public IUnmanagedTransporterToken AsUnmanaged() =>
            GetTransporterToken(() => new UnmanagedTransporterToken(_transporter));

        private T GetTransporterToken<T>(Func<T> createTransporterToken)
        {
            if (_transporter == null)
                throw new ObjectDisposedException(nameof(TransporterRegistry));

            var result = createTransporterToken();
            _transporter = null;
            _context = null;
            return result;
        }
    }

    internal sealed class UnmanagedTransporterToken : TransporterTokenBase, IUnmanagedTransporterToken
    {
        public UnmanagedTransporterToken(TransporterBase transpoter)
            : base(transpoter)
        {
        }

        public void Start() => Transpoter.Start();
    }

    internal sealed class ManagedTransporterToken : TransporterTokenBase, IManagedTransporterToken
    {
        private TransferContext _context;

        public ManagedTransporterToken(TransporterBase transpoter, TransferContext context)
            : base(transpoter)
        {
            _context = context;
        }

        public void Ready() => Transpoter.Status = TransferStatus.Ready;

        public void AsNext() => _context.AsNext(Transpoter);

        public override void Dispose()
        {
            _context = null;
            base.Dispose();
        }
    }

    internal class TransporterTokenBase : IDisposable
    {
        protected TransporterBase Transpoter;

        public TransporterTokenBase(TransporterBase transpoter) => Transpoter = transpoter;

        public void Suspend() => Transpoter.Suspend();

        public virtual void Dispose()
        {
            if (Transpoter.Status == TransferStatus.Transferring)
                Transpoter.Suspend();

            Transpoter.Dispose();
            Transpoter = null;
        }
    }
}
