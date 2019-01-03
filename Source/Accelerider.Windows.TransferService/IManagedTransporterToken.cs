using System;

namespace Accelerider.Windows.TransferService
{
    public interface IManagedTransporterToken : IDisposable
    {
        void Suspend();

        void Ready();

        void AsNext();
    }
}
