using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.TransferService
{
    public interface IRemotePathProvider : IPersistable<IRemotePathProvider>
    {
        void Rate(string remotePath, double score);

        Task<string> GetAsync();
    }
}
