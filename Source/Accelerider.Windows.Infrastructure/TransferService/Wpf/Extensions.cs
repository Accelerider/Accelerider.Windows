using System.Windows.Threading;

namespace Accelerider.Windows.Infrastructure.TransferService.Wpf
{
    public static class Extensions
    {
        public static DownloaderBindable ToBindable(this IDownloader @this, Dispatcher uiDispatcher = null)
        {
            return new DownloaderBindable(@this, uiDispatcher);
        }

    }
}
