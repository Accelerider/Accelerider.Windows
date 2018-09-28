using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
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
            public IEnumerable<INetDiskUser> NetDiskUsers { get; set; } = Enumerable.Empty<INetDiskUser>();

            [JsonIgnore]
            public INetDiskUser CurrentNetDiskUser { get; set; }


            public void Save(string path) => File.WriteAllText(path, JsonConvert.SerializeObject(this, ConfigureFile.JsonSerializerSettings));
        }

        private static IContainer _container;
        private static INetDiskApi _netDiskApi;
        private static ExtensionCache _cache;
        private static string DataFile => Path.Combine(Directory.GetCurrentDirectory(), "apps", "NetDisk", "Users.json");

        public static void Initialize(IContainer container)
        {
            _container = container;
            _netDiskApi = container.Resolve<INetDiskApi>();
            if (!File.Exists(DataFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DataFile));
                new ExtensionCache().Save(DataFile);
            }
            _cache = JsonConvert.DeserializeObject<ExtensionCache>(File.ReadAllText(DataFile), ConfigureFile.JsonSerializerSettings);
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
            /*
            var netDiskInfos = await _netDiskApi.GetAllNetDisksAsync();
            _cache.NetDiskUsers = netDiskInfos?.Select(item => _container.Resolve<NetDiskUser>(new TypedParameter(typeof(NetDiskInfo), item)));
            _cache.CurrentNetDiskUser = _cache.NetDiskUsers?.FirstOrDefault();
			*/
            if (_cache.NetDiskUsers != null)
                foreach (var user in _cache.NetDiskUsers)
                    await user.RefreshUserInfoAsync();
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
            var list = _cache.NetDiskUsers.ToList();
            if (list.Any(v => v.Id == user.Id))
                return Task.FromResult(false);
            list.Add(user);
            _cache.NetDiskUsers = list;
            _cache.Save(DataFile);
            return Task.FromResult(true);
        }

        public static Task<bool> RemoveNetDiskUserAsync(this IAcceleriderUser @this, INetDiskUser user)
        {
            CheckNullObject(@this);
            if (_cache.NetDiskUsers.All(v => v.Id != user.Id))
                return Task.FromResult(false);
            var list = _cache.NetDiskUsers.ToList();
            list.RemoveAll(v => v.Id == user.Id);
            _cache.NetDiskUsers = list;
            _cache.Save(DataFile);
            return Task.FromResult(true);
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
