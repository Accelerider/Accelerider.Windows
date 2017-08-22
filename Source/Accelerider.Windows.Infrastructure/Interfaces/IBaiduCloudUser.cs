using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IBaiduCloudUser : INetDiskUser
    {
        Uri HeadImageUri { get; }

        string Nickname { get; }
    }
}
