using System;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public class TransferTaskStatusChangedEventArgs
    {
        public TransferTaskStatusChangedEventArgs(ITransferTaskToken token, TransferTaskStatusEnum oldStatus, TransferTaskStatusEnum newStatus)
        {
            if(!oldStatus.CanChangeTo(newStatus)) throw new InvalidOperationException($"{oldStatus} can not converted to {newStatus}");

            Token = token;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }

        public ITransferTaskToken Token { get; }
        public TransferTaskStatusEnum OldStatus { get; }
        public TransferTaskStatusEnum NewStatus { get; }
    }
}
