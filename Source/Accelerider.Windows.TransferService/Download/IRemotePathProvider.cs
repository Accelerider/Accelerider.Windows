using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.TransferService
{
    public interface IRemotePathProvider
    {
        void Rate(string remotePath, double score);

        Task<string> GetRemotePathAsync();
    }
}
