using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// A interface represents the repository of <see cref="ILocalDiskUser"/>.
    /// </summary>
    public interface ILocalDiskUserRepository : IRepository<ILocalDiskUser>
    {
        Task<ILocalDiskUser> SignInAsync(string userName, string password);
        Task SignOutAsync();
        Task SignUpAsync(string userName, string password);
    }
}
