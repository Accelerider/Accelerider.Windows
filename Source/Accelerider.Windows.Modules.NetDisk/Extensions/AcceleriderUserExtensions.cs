using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models;
using Autofac;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Extensions
{
    public static class AcceleriderUserExtensions
    {
        private class ExtensionCache
        {
            public IEnumerable<INetDiskUser> NetDiskUsers { get; set; }

			[JsonIgnore]
            public INetDiskUser CurrentNetDiskUser { get; set; }
        }

        private static IContainer _container;
        private static INetDiskApi _netDiskApi;
        private static ExtensionCache _cache;

        public static void Initialize(IContainer container)
        {
            _container = container;
            _netDiskApi = container.Resolve<INetDiskApi>();
            _cache = new ExtensionCache();
        }

        private static void CheckNullObject(object @object)
        {
            if (@object == null) throw new NullReferenceException();
        }

        // -------------------------------------------------------------------------------------

        public static async Task<bool> RefreshAsyncEx(this IAcceleriderUser @this)
        {
            CheckNullObject(@this);

            var result = await @this.RefreshAsync();
            var netDiskInfos = await _netDiskApi.GetAllNetDisksAsync();
            _cache.NetDiskUsers = netDiskInfos?.Select(item => _container.Resolve<NetDiskUser>(new TypedParameter(typeof(NetDiskInfo), item)));
            _cache.CurrentNetDiskUser = _cache.NetDiskUsers?.FirstOrDefault();
            return result;
        }

        public static IEnumerable<INetDiskUser> GetNetDiskUsers(this IAcceleriderUser @this)
        {
            CheckNullObject(@this);
            return _cache.NetDiskUsers;
        }

        public static Task<bool> AddNetDiskUserAsync(this IAcceleriderUser @this, INetDiskUser user)
        {
            CheckNullObject(@this);
            //_netDiskApi.AddNetDiskAsync()
            throw new NotImplementedException();
        }

        public static async Task<bool> RemoveNetDiskUserAsync(this IAcceleriderUser @this, INetDiskUser user)
        {
            CheckNullObject(@this);
            try
            {
                await _netDiskApi.RemoveNetDiskByIdAsync(user.Id);
                return true;
            }
            catch (Exception e)
            {
                // TODO: Logging
                return false;
            }
        }

        public static INetDiskUser GetCurrentNetDiskUser(this IAcceleriderUser @this)
        {
            CheckNullObject(@this);
            return _cache.CurrentNetDiskUser;
        }

        public static void SetCurrentNetDiskUser(this IAcceleriderUser @this, INetDiskUser value)
        {
            CheckNullObject(@this);
            _cache.CurrentNetDiskUser = value;
        }

        // -------------------------------------------------------------------------------------
        public static IList<TransferItem> GetDonwloadItems(this IAcceleriderUser @this)
        {
            CheckNullObject(@this);
            throw new NotImplementedException();
        }

        public static IList<TransferItem> GetUploadItems(this IAcceleriderUser @this)
        {
            CheckNullObject(@this);
            throw new NotImplementedException();
        }

        public static IList<ITransferredFile> GetDownloadedFiles(this IAcceleriderUser @this)
        {
            CheckNullObject(@this);
            throw new NotImplementedException();
        }

        public static IList<ITransferredFile> GetUploadedFiles(this IAcceleriderUser @this)
        {
            CheckNullObject(@this);
            throw new NotImplementedException();
        }
    }
}
