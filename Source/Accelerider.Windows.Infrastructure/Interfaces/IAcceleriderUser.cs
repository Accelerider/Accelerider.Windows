using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IAcceleriderUser
    {
        // Accelerider account system -----------------------------------------------------------
        Task<string> SignUpAsync(string username, string password, string licenseCode);

        Task<string> SignInAsync(string username, string password);

        Task<bool> SignOutAsync();

        // Accelerider Services -----------------------------------------------------------------
        ITransferTaskToken Upload(FileLocation from, FileLocation to);

        Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null);

        //Task<ILazyTreeNode<INetDiskFile>> GetSharedFileRootAsync(ISharedFile shareSummary);

        // Operates sub-account (cloud account) -------------------------------------------------
        INetDiskUser CurrentNetDiskUser { get; set; }

        IReadOnlyList<INetDiskUser> NetDiskUsers { get; }

        Task<bool> AddNetDiskUserAsync(INetDiskUser user);

        Task<bool> RemoveNetDiskUserAsync(INetDiskUser user);

        // Gets transfer tasks or files ---------------------------------------------------------------------
        IReadOnlyCollection<ITransferTaskToken> GetDownloadingTasks();

        IReadOnlyCollection<ITransferTaskToken> GetUploadingTasks();

        IReadOnlyCollection<ITransferedFile> GetDownloadedFiles();

        IReadOnlyCollection<ITransferedFile> GetUploadedFiles();
    }
}
