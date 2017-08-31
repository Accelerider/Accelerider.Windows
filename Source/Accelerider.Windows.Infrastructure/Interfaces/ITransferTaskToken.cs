using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface ITransferTaskToken : IEquatable<ITransferTaskToken>
    {
        event EventHandler<TransferTaskStatusChangedEventArgs> TransferTaskStatusChanged;

        TransferTaskStatusEnum TransferTaskStatus { get; }

        IDiskFile FileInfo { get; }

        DataSize Progress { get; }

        Task<bool> PauseAsync();

        Task<bool> StartAsync(bool force = false);

        Task<bool> CancelAsync();
    }
}
