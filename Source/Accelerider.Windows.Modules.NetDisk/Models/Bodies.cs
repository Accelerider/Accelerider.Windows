namespace Accelerider.Windows.Modules.NetDisk.Models
{
    internal class CloudTaskPayload
    {
    }

    internal class FileUpdatePayload
    {
    }

    internal class FilePayload : FileUpdatePayload
    {
    }

    internal abstract class NetDiskAuthPayload
    {
    }

    internal class BaiduNetDiskAuthPayload : NetDiskAuthPayload
    {

    }

    internal class OneDriveAuthPayload : NetDiskAuthPayload
    {

    }
}
