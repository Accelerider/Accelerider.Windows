using Accelerider.Windows.Infrastructure.TransferService;
using Accelerider.Windows.Infrastructure.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public class TransferItem
    {
        public INetDiskUser Owner { get; }

        public INetDiskFile File { get; }

        public BindableDownloader Transporter { get; }

        public IManagedTransporterToken ManagedToken { get; }
    }
}