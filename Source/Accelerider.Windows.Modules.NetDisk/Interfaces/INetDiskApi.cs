using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Models;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces
{
    internal interface INetDiskApi
    {
        [Post("")]
        Task AddNetDiskAsync([Body] NetDiskAuthInfoBody netDiskInfo);

        [Delete("/{id}")]
        Task RemoveNetDiskByIdAsync(long id);

        [Get("/{id}")]
        Task<NetDiskMetaData> GetNetDiskByIdAsync(long id);

        [Get("/children")]
        Task<IList<NetDiskMetaData>> GetAllNetDisksAsync();

        // Accelerider Cloud tasks --------------------------------------------------------------------------
        [Post("/0/tasks")]
        Task AddCloudTaskAsync([Body] CloudTaskInfoBody cloudTaskInfo);

        [Delete("/0/tasks/{id}")]
        Task<CloudTaskMetadata> RemoveCloudTaskByIdAsync(long id);

        [Get("/0/tasks/{id}")]
        Task<CloudTaskMetadata> GetCloudTaskByIdAsync(long id);

        [Get("/0/tasks/children")]
        Task<CloudTaskMetadata> GetAllCloudTasksAsync();

        // Files --------------------------------------------------------------------------------------------
        [Post("/{netDiskId}/files")]
        Task<FileMetadata> AddFileAsync(long netDiskId, [Body] FileInfoBody fileInfo);

        [Delete("/{netDiskId}/files/{fileId}")]
        Task RemoveFileByIdAsync(long netDiskId, long fileId);

        [Patch("/{netDiskId}/files/{fileId}")]
        Task UpdateFileAsync(long netDiskId, long fileId, [Body] FileUpdateInfoBody fileInfo);

        [Get("/{netDiskId}/files/{fileId}")]
        Task<FileMetadata> GetFileByIdAsync(long netDiskId, long fileId);

        [Get("/{netDiskId}/files/{fileId}/children")]
        Task<IList<FileMetadata>> GetFileChildrenByIdAsync(long netDiskId, long fileId);

        [Get("/{netDiskId}/files/{fileType}")]
        Task<IList<FileMetadata>> GetAllFilesByTypeAsync(long netDiskId, SpecifiedFileType fileType);

        [Put("/{netDiskId}/files/{fileId}/content")]
        Task UploadFileAsync(long netDiskId, long fileId, [Body] Stream fileStream);

        [Get("/{netDiskId}/files/{fileId}/content")]
        Task<Stream> DownloadFileAsync(long netDiskId, long fileId);
    }
}
