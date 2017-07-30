using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface ITransferTaskToken : IEquatable<ITransferTaskToken>
    {
        event EventHandler<TransferStateChangedEventArgs> TransferStateChanged;

        TransferStateEnum TransferState { get; }

        IDiskFile FileInfo { get; }

        DataSize Progress { get; }

        Task<bool> PauseAsync();

        Task<bool> RestartAsync();

        Task<bool> CancelAsync();
    }
}
