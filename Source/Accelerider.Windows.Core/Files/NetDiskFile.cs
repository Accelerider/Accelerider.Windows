using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files
{
    public class NetDiskFile :DiskFileBase, INetDiskFile
    {
        public DateTime CreatedTime { get; set; }

        public DateTime ModifiedTime { get; set; }

        public override Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
