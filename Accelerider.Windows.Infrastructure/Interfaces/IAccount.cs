using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAccount
    {
        Task<bool> SignInAsync();
        Task<bool> SignOutAsync();

    }
}
