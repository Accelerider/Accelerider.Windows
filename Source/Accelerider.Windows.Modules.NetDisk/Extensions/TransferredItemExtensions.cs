using System;
using System.IO;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.TransferService.WpfInteractions;

namespace Accelerider.Windows.Modules.NetDisk
{
    public static class TransferredItemExtensions
    {
        private class TransferredFileImpl : ILocalDiskFile
        {
            public TransferredFileImpl(string localPath)
            {
                var fileInfo = new System.IO.FileInfo(localPath);

                Exists = fileInfo.Exists;
                if (fileInfo.Exists)
                {
                    Path = localPath;
                    Type = new FileLocator(localPath).GetFileType();
                    Size = fileInfo.Length;
                    CompletedTime = fileInfo.LastWriteTime;
                }
            }

            public FileType Type { get; }

            public FileLocator Path { get; }

            public DisplayDataSize Size { get; }

            public bool Exists { get; }

            public DateTime CompletedTime { get; }

            public Task<bool> DeleteAsync()
            {
                throw new NotImplementedException();
            }
        }

        public static ILocalDiskFile ToTransferredFile(this string @this)
        {
            return new TransferredFileImpl(@this);
        }
    }
}
