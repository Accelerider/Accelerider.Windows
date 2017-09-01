using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface ITransferTaskToken : IEquatable<ITransferTaskToken>
    {
        event EventHandler<TransferTaskStatusChangedEventArgs> TransferTaskStatusChanged;


        string OwnerName { get; }

        TransferTaskStatusEnum TransferTaskStatus { get; }

        DataSize Progress { get; }


        IDiskFile FileInfo { get; }

        ITransferedFile GetTransferedFile();


        Task<bool> PauseAsync();

        Task<bool> StartAsync(bool force = false);

        Task<bool> CancelAsync();
    }
}
