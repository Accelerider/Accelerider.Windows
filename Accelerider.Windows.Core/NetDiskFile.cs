using System;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    public class NetDiskFile : INetDiskFile
    {
        public DateTime CreatedTime { get; set; }

        public DateTime ModifiedTime { get; set; }

        public FileLocation FilePath { get; set; }

        public DataSize FileSize { get; set; }

        public long FileId { get; set; }

        public FileTypeEnum FileType { get; set; }

        public ITransferTaskToken Download()
        {
            throw new NotImplementedException();
        }
    }
}
