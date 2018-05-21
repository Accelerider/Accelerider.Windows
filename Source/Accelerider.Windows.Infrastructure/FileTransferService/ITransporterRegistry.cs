using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public interface ITransporterRegistry
    {
        IManagedTransporterToken AsManaged();

        IUnmanagedTransporterToken AsUnmanaged();
    }

    public interface IUnmanagedTransporterToken : IDisposable
    {
        /// <summary>
        /// Force start or restart the task, which may lead to transfer 
        /// from <see cref="TransferStatus.Suspended"/> / <see cref="TransferStatus.Faulted"/> / 
        /// <see cref="TransferStatus.Ready"/> to <see cref="TransferStatus.Transferring"/>. 
        /// </summary>
        void Start();

        /// <summary>
        /// Suspends the task, which may lead to transfer 
        /// from <see cref="TransferStatus.Ready"/> / <see cref="TransferStatus.Transferring"/> to <see cref="TransferStatus.Suspended"/>.
        /// </summary>
        void Suspend();
    }

    public interface IManagedTransporterToken : IDisposable
    {
        void Ready();

        void Suspend();

        void AsNext();
    }
}
