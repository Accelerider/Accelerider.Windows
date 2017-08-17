using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAcceleriderUser
    {
        // Operates sub-account (cloud account) -------------------------------------------------
        INetDiskUser CurrentNetDiskUser { get; set; }

        IReadOnlyList<INetDiskUser> NetDiskUsers { get; }

        ITransferTaskToken Upload(FileLocation filePath);

        Task<bool> AddNetDiskUserAsync(INetDiskUser user);

        Task<bool> RemoveNetDiskUserAsync(INetDiskUser user);

        // Accelerider account system -----------------------------------------------------------
        Task<string> SignUpAsync(string username, string password, string licenseCode);

        Task<string> SignInAsync(string username, string password);

        Task<bool> SignOutAsync();

        // Gets local files ---------------------------------------------------------------------
        IReadOnlyCollection<ITransferedFile> GetDownloadedFiles();

        IReadOnlyCollection<ITransferedFile> GetUploadedFiles();
    }
}
