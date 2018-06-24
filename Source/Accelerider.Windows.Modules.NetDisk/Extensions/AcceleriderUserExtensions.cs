using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.Extensions
{
    public static class AcceleriderUserExtensions
    {
        private static IUnityContainer Container { get; set; }

        // -------------------------------------------------------------------------------------
        public static IEnumerable<INetDiskUser> GetNetDiskUsers(this IAcceleriderUser @this)
        {
            throw new NotImplementedException();
        }

        public static Task<bool> AddNetDiskUserAsync(this IAcceleriderUser @this, INetDiskUser user)
        {
            throw new NotImplementedException();
        }

        public static Task<bool> RemoveNetDiskUserAsync(this IAcceleriderUser @this, INetDiskUser user)
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

        // -------------------------------------------------------------------------------------
        public static IList<TransferItem> GetDonwloadItems(this IAcceleriderUser @this)
        {
            throw new NotImplementedException();
        }

        public static IList<TransferItem> GetUploadItems(this IAcceleriderUser @this)
        {
            throw new NotImplementedException();
        }

        public static IList<ITransferredFile> GetDownloadedFiles(this IAcceleriderUser @this)
        {
            throw new NotImplementedException();
        }

        public static IList<ITransferredFile> GetUploadedFiles(this IAcceleriderUser @this)
        {
            throw new NotImplementedException();
        }
    }
}
