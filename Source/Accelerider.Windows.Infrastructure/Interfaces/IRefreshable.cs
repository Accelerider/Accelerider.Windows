using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IRefreshable
    {
        /// <summary>
        /// Try to refresh the object.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> type indicating whether the data was successfully fetched.</returns>
        Task<bool> RefreshAsync();
    }
}
