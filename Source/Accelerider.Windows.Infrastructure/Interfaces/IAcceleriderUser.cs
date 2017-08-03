using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAcceleriderUser
    {
        ICollection<INetDiskUser> NetDiskUsers { get; }

        INetDiskUser CurrentNetDiskUser { get; set; }

        Task<bool> AddNetDiskUserAsync(INetDiskUser user);

        Task<bool> RemoveNetDiskUserAsync(INetDiskUser user);

        ITransferTaskToken UploadToFilePlaza(FileLocation filePath);

        Task<bool> LoginAsync(string username, string password);

        Task<bool> SignOutAsync();

        ICollection<ITransferedFile> GetDownloadedFiles();

        ICollection<IDiskFile> GetDownloadingFiles();

        ICollection<ITransferedFile> GetUploadedFiles();

        ICollection<IDiskFile> GetUploadingFiles();
    }
}
