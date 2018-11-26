using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Modules.NetDisk
{
    public static class DownloaderBuilderExtensions
    {
        public static IDownloaderBuilder UseBaiduCloudConfigure(this IDownloaderBuilder @this) => @this
            .UseDefaultConfigure();

        public static IDownloaderBuilder UseOneDriveConfigure(this IDownloaderBuilder @this) => @this
            .UseDefaultConfigure();

        public static IDownloaderBuilder UseOneOneFiveCloudConfigure(this IDownloaderBuilder @this) => @this
            .UseDefaultConfigure();

        public static IDownloaderBuilder UseSixCloudConfigure(this IDownloaderBuilder @this) => @this
            .UseDefaultConfigure();
    }
}
