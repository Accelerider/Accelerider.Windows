using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Enumerations;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces
{
    public interface INetDiskUser
    {
        // User Information ----------------------------------------------------------------

        string Username { get; }

        DataSize TotalCapacity { get; }

        DataSize UsedCapacity { get; }

        Task<bool> RefreshUserInfoAsync();

        // Gets net-disk files -------------------------------------------------------------

        Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

        Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();

        Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();

        Task<Dictionary<FileQueries, IEnumerable<IFile>>> GetFileCategoryDictionaryAsync();

        // Transport operations ------------------------------------------------------------


    }
}
