using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Events
{
    public class TransferStateChangedEvent : EventBase<TransferStateChangedEventArgs> { }

    public class DownloadTaskCreatedEvent : EventBase<IReadOnlyCollection<ITransferTaskToken>> { }

}
