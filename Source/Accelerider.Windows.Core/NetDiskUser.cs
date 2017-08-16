using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine;
using Accelerider.Windows.Core.Files;
using Accelerider.Windows.Core.MockData;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal class NetDiskUser : INetDiskUser
    {
        public Uri HeadImageUri { get; set; }

        public string Username { get; set; }

        public string Nickname { get; set; }

        public DataSize TotalCapacity => new DataSize(5, SizeUnitEnum.T);

        public DataSize UsedCapacity => new DataSize(2.34, SizeUnitEnum.T);

        public IReadOnlyCollection<ITransferTaskToken> GetDownloadingFiles()
        {
            return new List<ITransferTaskToken>();
        }

        public IReadOnlyCollection<ITransferTaskToken> GetUploadingFiles()
        {
            return new List<ITransferTaskToken>();
        }


        public async Task<ITreeNodeAsync<INetDiskFile>> GetNetDiskFileRootAsync()
        {
            return await GetNetDiskFileTreeAsyncMock();
        }

        public async Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            return await GetDeletedFilesAsyncMock();
        }

        public async Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            return await GetSharedFilesAsyncMock();
        }


        public ITransferTaskToken UploadAsync(FileLocation from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ITreeNodeAsync<INetDiskFile> fileNode, FileLocation downloadFolder = null)
        {
            return await DownloadAsyncMock(fileNode);
        }

        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        // TODO: Remove mock data
        #region Demo data

        public string FilePathMock = null;

        private async Task<ITreeNodeAsync<INetDiskFile>> GetNetDiskFileTreeAsyncMock()
        {
            await Task.Delay(1000);
            var tree = new TreeNodeAsync<INetDiskFile>(new NetDiskFile { FilePath = new FileLocation(FilePathMock) })
            {
                ChildrenProvider = async parent =>
                {
                    await Task.Delay(1);
                    if (!Directory.Exists(parent.FilePath)) return null;
                    var filePaths = Directory.GetFiles(parent.FilePath.ToString());
                    var directoriePaths = Directory.GetDirectories(parent.FilePath.ToString());
                    return from filePath in directoriePaths.Union(filePaths)
                        where File.Exists(filePath) || Directory.Exists(filePath)
                        select new NetDiskFile
                        {
                            FilePath = filePath,
                            FileSize = File.Exists(filePath)
                                ? new DataSize(new FileInfo(filePath).Length)
                                : default(DataSize),
                            ModifiedTime = new FileInfo(filePath).LastWriteTime
                        };
                }
            };
            return tree;
        }

        private async Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsyncMock(ITreeNodeAsync<INetDiskFile> fileNode, FileLocation downloadFolder = null)
        {
            return (from file in await fileNode.FlattenAsync()
                    where file.Content.FileType != FileTypeEnum.FolderType
                    select new TransferTaskTokenMockData(file.Content))
                .ToList();
        }

        private async Task<IEnumerable<ISharedFile>> GetSharedFilesAsyncMock()
        {
            await Task.Delay(100);
            var rand = new Random();
            var filePaths = Directory.GetFiles(FilePathMock);
            var directoriePaths = Directory.GetDirectories(FilePathMock);
            return from filePath in directoriePaths.Union(filePaths)
                where File.Exists(filePath) || Directory.Exists(filePath)
                select new SharedFile
                {
                    Name = new FileLocation(filePath).FileName,
                    DownloadedNumber = rand.Next(0, 1000),
                    SavedNumber = rand.Next(0, 1000),
                    VisitedNumber = rand.Next(0, 1000),
                    SharedTime = new FileInfo(filePath).LastWriteTime,
                    ShareLink = new Uri(@"https://pan.baidu.com/s/1jGE6mpC")
                };
        }

        private async Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsyncMock()
        {
            await Task.Delay(100);
            var rand = new Random();
            var filePaths = Directory.GetFiles(FilePathMock);
            var directoriePaths = Directory.GetDirectories(FilePathMock);
            return from filePath in directoriePaths.Union(filePaths)
                where File.Exists(filePath) || Directory.Exists(filePath)
                select new DeletedFile
                {
                    FilePath = filePath,
                    LeftDays = rand.Next(1, 11),
                    FileSize = File.Exists(filePath) ? new DataSize(new FileInfo(filePath).Length) : default(DataSize),
                    DeletedTime = new FileInfo(filePath).LastWriteTime
                };
        }
        #endregion
    }
}