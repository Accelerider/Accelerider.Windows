using System;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces
{
    public interface IBaiduCloudUser : INetDiskUser
    {
        Uri HeadImageUri { get; }

        string Nickname { get; }
    }
}
