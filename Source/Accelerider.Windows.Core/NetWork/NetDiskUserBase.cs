using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.NetWork
{
    public abstract class NetDiskUserBase : INetDiskUser
    {
        public virtual string Username { get; protected set; }
        public virtual string UserId { get; protected set; }
        public virtual DataSize TotalCapacity { get; protected set; }
        public virtual DataSize UsedCapacity { get; protected set; }


        public abstract Task<bool> RefreshUserInfoAsync();
        public abstract ITransferTaskToken UploadAsync(FileLocation @from, FileLocation to);

        public async Task DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder,Action<ITransferTaskToken> action)
        {
            if (fileNode.Content.FileType == FileTypeEnum.FolderType)
            {
                var redundantPathLength = fileNode.Content.FilePath.FolderPath.Length + 1;
                await fileNode.ForEachAsync(file =>
                {
                    if (file.FileType == FileTypeEnum.FolderType) return;
                    var subPath = file.FilePath.FullPath.Substring(redundantPathLength);
                    FileLocation downloadPath = Path.Combine(downloadFolder, subPath);
                    if (!Directory.Exists(downloadPath.FolderPath))
                        Directory.CreateDirectory(downloadPath.FolderPath);
                    action(DownloadTaskManager.Manager.Add(new DownloadTaskItem()
                    {
                        FilePath = file.FilePath,
                        DownloadPath = downloadPath,
                        FromUser = UserId,
                        NetDiskFile = new DownloadTaskFile()
                        {
                            FilePath = file.FilePath,
                            FileSize = file.FileSize,
                            FileType = file.FileType
                        },
                        Completed = false
                    }));
                });
            }
            else
            {
                action(DownloadTaskManager.Manager.Add(new DownloadTaskItem()
                {
                    FilePath = fileNode.Content.FilePath,
                    DownloadPath = Path.Combine(downloadFolder, fileNode.Content.FilePath.FileName),
                    FromUser = UserId,
                    NetDiskFile = new DownloadTaskFile()
                    {
                        FilePath = fileNode.Content.FilePath,
                        FileSize = fileNode.Content.FileSize,
                        FileType = fileNode.Content.FileType
                    },
                    Completed = false
                }));
            }
        }
        public abstract Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null);
        public abstract Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync();
        public abstract Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();
        public abstract Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();
    }
}
