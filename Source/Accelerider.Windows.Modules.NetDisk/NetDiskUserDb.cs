using System.Collections.Generic;
using Accelerider.Windows.Modules.NetDisk.Models;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class NetDiskUserDb : BindableBase
    {
        private List<INetDiskUser> _netDiskUsers;

        public List<INetDiskUser> NetDiskUsers
        {
            get => _netDiskUsers;
            set => SetProperty(ref _netDiskUsers, value);
        }
    }
}
