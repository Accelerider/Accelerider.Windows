using Accelerider.Windows.Infrastructure.Interfaces;
using Prism.Events;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class CurrentNetDiskUserChangedEvent : PubSubEvent<INetDiskUser> { }


    public class IsLoadingFilesChangedEvent : PubSubEvent<bool> { }

    public class IsLoadingMainWindowEvent : PubSubEvent<bool> { }
}
