using System.Threading.Tasks;

namespace Accelerider.Windows.TransferService
{
    public interface IRemotePathProvider
    {
        void Score(string remotePath, double score);

        Task<string> GetRemotePathAsync();
    }
}
