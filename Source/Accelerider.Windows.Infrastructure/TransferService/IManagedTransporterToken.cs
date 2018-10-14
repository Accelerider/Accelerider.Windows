using System;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IManagedTransporterToken : IDisposable
    {
        void Suspend();

        void Ready();

        void AsNext();
    }
}
