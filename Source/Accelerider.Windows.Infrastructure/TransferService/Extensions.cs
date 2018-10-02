using System.Linq;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class Extensions
    {
        public static long GetCompletedSize(this IDownloader @this)
        {
            return @this.BlockContexts?.Values.Sum(item => item.CompletedSize) ?? 0;
        }
    }
}
