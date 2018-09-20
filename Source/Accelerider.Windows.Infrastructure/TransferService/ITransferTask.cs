using System;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface ITransferTask
    {
        IObservable<ITransferContext> Activate();

        void Suspend();

        IManagedTransferTask AsManaged(string queueName = null);
    }

    public interface IManagedTransferTask : ITransferTask
    {
        void AsNext();
    }
}
