using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.TransferService.WpfInteractions
{
    public static class Extensions
    {
        public static BindableDownloader ToBindable(this IDownloader @this, Dispatcher uiDispatcher = null)
        {
            return new BindableDownloader(@this, uiDispatcher);
        }

    }
}
