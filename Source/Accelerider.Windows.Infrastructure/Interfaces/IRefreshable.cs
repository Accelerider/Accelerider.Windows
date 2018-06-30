using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IRefreshable
    {
        Task<bool> RefreshAsync();
    }
}
