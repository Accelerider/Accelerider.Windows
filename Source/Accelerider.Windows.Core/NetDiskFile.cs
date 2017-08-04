using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    public class NetDiskFile :DiskFileBase, INetDiskFile
    {
        public DateTime CreatedTime { get; set; }

        public DateTime ModifiedTime { get; set; }

        public ITransferTaskToken Download()
        {
            throw new NotImplementedException();
        }
    }
}
