using System.Windows.Threading;
using Accelerider.Windows.Infrastructure.TransferService;

namespace Accelerider.Windows.Infrastructure.WpfInteractions
{
    public static class Extensions
    {
        public static BindableDownloader ToBindable(this IDownloader @this, Dispatcher uiDispatcher = null)
        {
            return new BindableDownloader(@this, uiDispatcher ?? Dispatcher.CurrentDispatcher);
        }

    }
}
