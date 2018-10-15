using System.Windows.Threading;

namespace Accelerider.Windows.Infrastructure.TransferService.WpfInteractions
{
    public static class Extensions
    {
        public static BindableDownloader ToBindable(this IDownloader @this, Dispatcher uiDispatcher = null)
        {
            return new BindableDownloader(@this, uiDispatcher ?? Dispatcher.CurrentDispatcher);
        }

    }
}
