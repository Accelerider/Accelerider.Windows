using System;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public class NetDiskInfo : BindableBase, INetDiskInfo
    {
        private string _id;
        private string _username;
        private Uri _avatar;
        private long _usedCapacity;
        private long _totalCapacity;

        public string Id
        {
            get => _id;
            protected set => SetProperty(ref _id, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public Uri Avatar
        {
            get => _avatar;
            set => SetProperty(ref _avatar, value);
        }

        public long UsedCapacity
        {
            get => _usedCapacity;
            protected set => SetProperty(ref _usedCapacity, value);
        }

        public long TotalCapacity
        {
            get => _totalCapacity;
            protected set => SetProperty(ref _totalCapacity, value);
        }

        public virtual Task<bool> RefreshAsync() => Task.FromResult(false);
    }
}
