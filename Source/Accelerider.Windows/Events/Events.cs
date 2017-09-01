using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Events
{
    public class TaskCreatedEvent : EventBase<IReadOnlyCollection<ITransferTaskToken>> { }

    public class DownloadTaskCreatedEvent : TaskCreatedEvent { }

    public class UploadTaskCreatedEvent : TaskCreatedEvent { }


    public class TaskEndEvent : EventBase<ITransferTaskToken> { }

    public class DownloadTaskEndEvent : TaskEndEvent { }

    public class UploadTaskEndEvent : TaskEndEvent { }


    public class CurrentNetDiskUserChangedEvent : EventBase<INetDiskUser> { }


    public class IsLoadingFilesChangedEvent : EventBase<bool> { }

    public class IsLoadingMainWindowEvent : EventBase<bool> { }
}
