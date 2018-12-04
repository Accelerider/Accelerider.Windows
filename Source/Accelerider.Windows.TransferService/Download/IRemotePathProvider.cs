using System.Threading.Tasks;

namespace Accelerider.Windows.TransferService
{
    public interface IRemotePathProvider
    {
        void Rate(string remotePath, double score);

        Task<string> GetRemotePathAsync();
    }
}
