using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAcceleriderUser
    {
        INetDiskUser CurrentNetDiskUser { get; set; }
        IReadOnlyCollection<INetDiskUser> NetDiskUsers { get; }

        // Accelerider account system -----------------------------------------------------------
        Task<bool> LoginAsync(string username, string password);

        Task<bool> SignOutAsync();

        // Operates sub-account (cloud account) -------------------------------------------------
        Task<bool> AddNetDiskUserAsync(INetDiskUser user);

        Task<bool> RemoveNetDiskUserAsync(INetDiskUser user);

        ITransferTaskToken Upload(FileLocation filePath);

        IReadOnlyCollection<ITransferedFile> GetDownloadedFiles();

        IReadOnlyCollection<ITransferedFile> GetUploadedFiles();
    }
}
