using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Events
{
    public class CurrentNetDiskUserChangedEvent : EventBase<INetDiskUser> { }


    public class IsLoadingFilesChangedEvent : EventBase<bool> { }

    public class IsLoadingMainWindowEvent : EventBase<bool> { }
}
