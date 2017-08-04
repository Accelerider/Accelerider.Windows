using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAcceleriderUser
    {
        INetDiskUser CurrentNetDiskUser { get; set; }
        IReadOnlyCollection<INetDiskUser> NetDiskUsers { get; }


        Task<bool> LoginAsync(string username, string password);
        Task<bool> SignOutAsync();

        Task<bool> AddNetDiskUserAsync(INetDiskUser user);
        Task<bool> RemoveNetDiskUserAsync(INetDiskUser user);

        ITransferTaskToken UploadToFilePlaza(FileLocation filePath);

        #region Gets local files
        IEnumerable<ITransferedFile> GetDownloadedFiles();
        IEnumerable<IDiskFile> GetDownloadingFiles();
        IEnumerable<ITransferedFile> GetUploadedFiles();
        IEnumerable<IDiskFile> GetUploadingFiles();
        #endregion
    }
}
