using System;
using Prism.Events;

namespace BaiduPanDownloadWpf.Infrastructure.Events
{
    public class NetDiskUserSwitchedEvent : PubSubEvent<NetDiskUserSwitchedEventArgs>
    {
    }

    public class NetDiskUserSwitchedEventArgs : EventArgsBase
    {
        public NetDiskUserSwitchedEventArgs(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}
