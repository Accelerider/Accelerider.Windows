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
            throw new NotImplementedException();
        }

        public Task<bool> RemoveNetDiskUserAsync(INetDiskUser user)
        {
            throw new NotImplementedException();
        }

        public ITransferTaskToken UploadToFilePlaza(FileLocation filePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SignOutAsync()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ITransferedFile> GetDownloadedFiles()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ITransferTaskToken> GetDownloadingFiles()
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
            return (from file in temp select new TransferTaskTokenMockData(file)).ToList();
        }


        public IReadOnlyCollection<ITransferedFile> GetUploadedFiles()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ITransferTaskToken> GetUploadingFiles()
        {
            throw new NotImplementedException();
        }
    }
}