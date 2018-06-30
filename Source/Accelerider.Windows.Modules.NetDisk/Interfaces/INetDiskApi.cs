using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Models;
using Refit;
using FileInfo = Accelerider.Windows.Modules.NetDisk.Models.FileInfo;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces
{
    internal interface INetDiskApi
    {
        [Post("")]
        Task AddNetDiskAsync([Body] NetDiskUserData netDiskInfo);

        [Delete("/{id}")]
        Task RemoveNetDiskByIdAsync(long id);

        [Get("/{id}")]
        Task<NetDiskInfo> GetNetDiskByIdAsync(long id);

        [Get("/children")]
        Task<IList<NetDiskInfo>> GetAllNetDisksAsync();

        // Accelerider Cloud tasks --------------------------------------------------------------------------
        [Post("/0/tasks")]
        Task AddCloudTaskAsync([Body] CloudTaskData cloudTaskInfo);

        [Delete("/0/tasks/{id}")]
        Task<CloudTaskInfo> RemoveCloudTaskByIdAsync(long id);

        [Get("/0/tasks/{id}")]
        Task<CloudTaskInfo> GetCloudTaskByIdAsync(long id);

        [Get("/0/tasks/children")]
        Task<CloudTaskInfo> GetAllCloudTasksAsync();

        // Files --------------------------------------------------------------------------------------------
        [Post("/{netDiskId}/files")]
        Task<FileInfo> AddFileAsync(long netDiskId, [Body] FileData fileInfo);

        [Delete("/{netDiskId}/files/{fileId}")]
        Task RemoveFileByIdAsync(long netDiskId, long fileId);

        [Patch("/{netDiskId}/files/{fileId}")]
        Task UpdateFileAsync(long netDiskId, long fileId, [Body] FileUpdateData fileInfo);

        [Get("/{netDiskId}/files/{fileId}")]
        Task<FileInfo> GetFileByIdAsync(long netDiskId, long fileId);

        [Get("/{netDiskId}/files/{fileId}/children")]
        Task<IList<FileInfo>> GetFileChildrenByIdAsync(long netDiskId, long fileId);

        [Get("/{netDiskId}/files/{fileType}")]
        Task<IList<FileInfo>> GetAllFilesByTypeAsync(long netDiskId, FileCategory fileType);

        [Put("/{netDiskId}/files/{fileId}/content")]
        Task UploadFileAsync(long netDiskId, long fileId, [Body] Stream fileStream);

        [Get("/{netDiskId}/files/{fileId}/content")]
        Task<Stream> DownloadFileAsync(long netDiskId, long fileId);
    }
}
