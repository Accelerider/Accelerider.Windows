using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.Modules.NetDisk.ViewModels;

using Unity;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.Infrastructure
{
    public static class AcceleriderUserExtensions
    {
        private class AcceleriderUserExtendedMembers
        {
            private static readonly ILogger Logger = DefaultLogger.Get(typeof(AcceleriderUserExtendedMembers));

            private readonly IDataRepository _repository;
            private readonly NetDiskUserDb _db;

            public List<INetDiskUser> NetDiskUsers => _db.NetDiskUsers;

            public INetDiskUser CurrentNetDiskUser { get; set; }

            public AcceleriderUserExtendedMembers(IDataRepository repository, NetDiskUserDb db)
            {
                if (db.NetDiskUsers == null) db.NetDiskUsers = new List<INetDiskUser>();
                _repository = repository;
                _db = db;
            }

            public void Save()
            {
                _repository.Save(_db);

                _db.NetDiskUsers
                    .ForEach(item => Logger.Info($"The user ({item.GetHashCode()}-{item.Username}) has been saved. "));
            }
        }

        private static AcceleriderUserExtendedMembers _extendedMembers;

        public static void Initialize(IUnityContainer container)
        {
            var repository = container.Resolve<IDataRepository>();

            _extendedMembers = new AcceleriderUserExtendedMembers(repository, repository.Get<NetDiskUserDb>());
        }

        // -------------------------------------------------------------------------------------

        public static event PropertyChangedEventHandler PropertyChanged;

        public static void Register(ViewModelBase viewModel)
        {
            PropertyChanged += viewModel.PropertyChangedHandler;
        }

        public static void Unregister(ViewModelBase viewModel)
        {
            PropertyChanged -= viewModel.PropertyChangedHandler;
        }

        public static async Task<bool> UpdateAsync(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            var result = await @this.RefreshAsync();
            foreach (var user in _extendedMembers.NetDiskUsers)
            {
                await user.RefreshAsync();
            }

            _extendedMembers.CurrentNetDiskUser = _extendedMembers.NetDiskUsers?.FirstOrDefault();

            return result;
        }

        public static AnyDriveAttachedProperties AttachedProperties(this IAcceleriderUser @this)
        {
            return new AnyDriveAttachedProperties(@this);
        }

        public static void SaveToLocalDisk(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            _extendedMembers.Save();
        }

        public static IEnumerable<INetDiskUser> GetNetDiskUsers(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            return _extendedMembers.NetDiskUsers;
        }

        public static bool AddNetDiskUser(this IAcceleriderUser @this, INetDiskUser value)
        {
            Guards.ThrowIfNull(@this);

            if (_extendedMembers.NetDiskUsers.Any(item => item.Id == value.Id)) return false;

            _extendedMembers.NetDiskUsers.Add(value);
            @this.SaveToLocalDisk();

            if (@this.GetCurrentNetDiskUser() == null)
            {
                @this.SetCurrentNetDiskUser(value);
            }

            RaisePropertyChanged();
            return true;
        }

        public static bool RemoveNetDiskUser(this IAcceleriderUser @this, INetDiskUser value)
        {
            Guards.ThrowIfNull(@this);

            var result = _extendedMembers.NetDiskUsers.RemoveAll(item => item.Id == value.Id) > 0;
            if (result)
            {
                @this.SaveToLocalDisk();
                RaisePropertyChanged();
            }
            return result;
        }

        public static INetDiskUser GetCurrentNetDiskUser(this IAcceleriderUser @this)
        {
            Guards.ThrowIfNull(@this);

            return _extendedMembers.CurrentNetDiskUser;
        }

        public static bool SetCurrentNetDiskUser(this IAcceleriderUser @this, INetDiskUser value)
        {
            Guards.ThrowIfNull(@this);

            if (EqualityComparer<INetDiskUser>.Default.Equals(_extendedMembers.CurrentNetDiskUser, value))
                return false;

            _extendedMembers.CurrentNetDiskUser = value;
            RaisePropertyChanged();
            return true;
        }

        // -------------------------------------------------------------------------------------

        private static void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            Guards.ThrowIfNull(propertyName);

            if (propertyName.StartsWith("Remove") || propertyName.StartsWith("Add"))
            {
                propertyName += "s";
            }

            propertyName = propertyName
                .Replace("Set", string.Empty)
                .Replace("Add", string.Empty)
                .Replace("Remove", string.Empty);

            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
