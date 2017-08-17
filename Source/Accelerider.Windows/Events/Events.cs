using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Events
{
    public class DownloadTaskCreatedEvent : EventBase<IReadOnlyCollection<ITransferTaskToken>> { }

    public class DownloadTaskTranferedEvent : EventBase<IDiskFile> { }

    public  class DownloadTaskCheckedEvent : EventBase<object> { }

    public class UploadTaskCreatedEvent : EventBase<IReadOnlyCollection<ITransferTaskToken>> { }

    public class UploadTaskCompletedEvent : EventBase<IDiskFile> { }

    public class CurrentNetDiskUserChangedEvent : EventBase<INetDiskUser> { }

}
