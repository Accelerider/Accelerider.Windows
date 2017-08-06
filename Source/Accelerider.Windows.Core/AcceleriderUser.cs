using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Core.Files;
using Accelerider.Windows.Core.MockData;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal class AcceleriderUser : IAcceleriderUser
    {
        public AcceleriderUser()
        {
            CurrentNetDiskUser = new NetDiskUser();
        }


        public IReadOnlyCollection<INetDiskUser> NetDiskUsers { get; private set; }
        public INetDiskUser CurrentNetDiskUser { get; set; }

        public Task<bool> AddNetDiskUserAsync(INetDiskUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveNetDiskUserAsync(INetDiskUser user)
        {
            throw new System.NotImplementedException();
        }

        public ITransferTaskToken UploadToFilePlaza(FileLocation filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> LoginAsync(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SignOutAsync()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITransferedFile> GetDownloadedFiles()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITransferTaskToken> GetDownloadingFiles()
        {
            var rand = new Random();
            const string folderPath = @"G:\Downloads";
            var temp = from filePath in Directory.GetFiles(folderPath)
                       select new DeletedFile
                       {
                           FilePath = new FileLocation(filePath),
                           LeftDays = rand.Next(1, 11),
                           FileSize = File.Exists(filePath) ? new DataSize(new FileInfo(filePath).Length) : default(DataSize),
                           DeletedTime = new FileInfo(filePath).LastWriteTime
                       };
            return from file in temp select new TransferTaskTokenMockData(file);
        }


        public IEnumerable<ITransferedFile> GetUploadedFiles()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITransferTaskToken> GetUploadingFiles()
        {
            throw new System.NotImplementedException();
        }
    }
}