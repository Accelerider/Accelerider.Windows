using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public class TransferItem
    {
        public INetDiskUser Owner { get; }

        public INetDiskFile File { get; }

        public TransporterReporter Reporter { get; }

        public IManagedTransporterToken ManagedToken { get; }
    }
}