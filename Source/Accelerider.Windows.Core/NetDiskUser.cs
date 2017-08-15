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
            return await GetNetDiskFileTreeInternalAsync();
        }

        private async Task<ITreeNodeAsync<INetDiskFile>> GetNetDiskFileTreeInternalAsync()
        {
            await Task.Delay(100);
            var tree = new TreeNodeAsync<INetDiskFile>(new NetDiskFile { FilePath = new FileLocation("G:\\") })
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

        public async Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            await Task.Delay(100);
            var rand = new Random();
            const string folderPath = "E:\\";
            var filePaths = Directory.GetFiles(folderPath);
            var directoriePaths = Directory.GetDirectories(folderPath);
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

        public async Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            await Task.Delay(100);
            var rand = new Random();
            const string folderPath = "E:\\";
            var filePaths = Directory.GetFiles(folderPath);
            var directoriePaths = Directory.GetDirectories(folderPath);
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

        public ITransferTaskToken UploadAsync(FileLocation @from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ITreeNodeAsync<INetDiskFile> fileNode)
        {
            return (from file in await fileNode.FlattenAsync()
                    where file.Content.FileType != FileTypeEnum.FolderType
                    select new TransferTaskTokenMockData(file.Content))
                    .ToList();
        }

        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }
    }
}