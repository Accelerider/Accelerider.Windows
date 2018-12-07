using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models;
using Newtonsoft.Json;
using Unity;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.Infrastructure
{
    public static class AcceleriderUserExtensions
    {
        private class AcceleriderUserExtendedMembers
        {
            public string Id { get; set; }

            public List<INetDiskUser> NetDiskUsers { get; } = new List<INetDiskUser>();

            [JsonIgnore]
            public INetDiskUser CurrentNetDiskUser { get; set; }


            public void Save(string path) => File.WriteAllText(path, this.ToJson());
        }

        private static readonly string UsersFilePathFormat = Path.Combine(Directory.GetCurrentDirectory(), "apps", "NetDisk", "{0}.users");

        private static IUnityContainer _container;
        private static INetDiskApi _netDiskApi;

        private static readonly IDictionary<string, AcceleriderUserExtendedMembers> ExtendedMembers =
            new ConcurrentDictionary<string, AcceleriderUserExtendedMembers>();

        public static void Initialize(IUnityContainer container)
        {
            _container = container;
            _netDiskApi = container.Resolve<INetDiskApi>();

            var usersDirectory = Path.GetDirectoryName(UsersFilePathFormat);

            if (!Directory.Exists(usersDirectory))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(usersDirectory);
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.GetFiles(usersDirectory)
                .Where(item => item.EndsWith(Path.GetExtension(UsersFilePathFormat)))
                .Select(item => File.ReadAllText(item).ToObject<AcceleriderUserExtendedMembers>())
                .ForEach(item => ExtendedMembers[item.Id] = item);
        }

        private static AcceleriderUserExtendedMembers GetExtendedMembers(this IAcceleriderUser user)
        {
            var result = ExtendedMembers.Get(user.Email);
            result.Id = user.Email;
            return result;
        }

        private static string GetUsersFilePath(this IAcceleriderUser user)
        {
            return string.Format(UsersFilePathFormat, user.Email);
        }

        // -------------------------------------------------------------------------------------

        public static async Task<bool> UpdateAsync(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            var result = await @this.RefreshAsync();
            var extendedMembers = @this.GetExtendedMembers();
            if (extendedMembers.NetDiskUsers != null)
            {
                foreach (var user in extendedMembers.NetDiskUsers)
                {
                    await user.RefreshUserInfoAsync();
                }
            }

            extendedMembers.CurrentNetDiskUser = extendedMembers.NetDiskUsers?.FirstOrDefault();

            return result;
        }

        public static IEnumerable<INetDiskUser> GetNetDiskUsers(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            return @this.GetExtendedMembers().NetDiskUsers;
        }

        public static bool AddNetDiskUser(this IAcceleriderUser @this, INetDiskUser user)
        {
            Guards.ThrowIfNull(@this);

            var extendedMembers = @this.GetExtendedMembers();

            if (extendedMembers.NetDiskUsers.Any(v => v.Id == user.Id)) return false;

            extendedMembers.NetDiskUsers.Add(user);
            extendedMembers.Save(@this.GetUsersFilePath());

            return true;
        }

        public static bool RemoveNetDiskUser(this IAcceleriderUser @this, INetDiskUser user)
        {
            Guards.ThrowIfNull(@this);

            return @this.GetExtendedMembers().NetDiskUsers.RemoveAll(item => item.Id == user.Id) > 0;
        }

        public static INetDiskUser GetCurrentNetDiskUser(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            return @this.GetExtendedMembers().CurrentNetDiskUser;
        }

        public static void SetCurrentNetDiskUser(this IAcceleriderUser @this, INetDiskUser value)
        {
            Guards.ThrowIfNull(@this);

            @this.GetExtendedMembers().CurrentNetDiskUser = value;
        }

        // -------------------------------------------------------------------------------------

        public static IReadOnlyCollection<TransferItem> GetDownloadItems(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            return @this.GetNetDiskUsers()
                .SelectMany(item => item.GetDownloadItems())
                .ToList()
                .AsReadOnly();
        }

        public static IList<ITransferredFile> GetDownloadedFiles(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);
            throw new NotImplementedException();
        }

        public static IList<ITransferredFile> GetUploadedFiles(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);
            throw new NotImplementedException();
        }
    }
}
