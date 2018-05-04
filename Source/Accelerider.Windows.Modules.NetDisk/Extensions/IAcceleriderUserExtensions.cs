using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Extensions
{
    public static class IAcceleriderUserExtensions
    {
        public static IEnumerable<INetDiskUser> GetNetDiskUsers(this IAcceleriderUser @this)
        {
            throw new NotImplementedException();
        }

        public static INetDiskUser GetCurrentNetDiskUser(this IAcceleriderUser @this)
        {
            throw new NotImplementedException();
        }

        public static void SetCurrentNetDiskUser(this IAcceleriderUser @this, INetDiskUser value)
        {
            throw new NotImplementedException();
        }


    }
}
