using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAcceleriderUser
    {
        // Operates sub-account (cloud account) -------------------------------------------------
        INetDiskUser CurrentNetDiskUser { get; set; }

        IReadOnlyList<INetDiskUser> NetDiskUsers { get; }

        Task<bool> RemoveNetDiskUserAsync(INetDiskUser user);

        ITransferTaskToken Upload(FileLocation filePath);

        // Accelerider account system -----------------------------------------------------------
        Task<bool> LoginAsync(string username, string password);

        Task<bool> SignOutAsync();

        Task<bool> AddNetDiskUserAsync(INetDiskUser user);

        // Gets local files ---------------------------------------------------------------------
        IReadOnlyCollection<ITransferedFile> GetDownloadedFiles();

        IReadOnlyCollection<ITransferedFile> GetUploadedFiles();
    }
}
