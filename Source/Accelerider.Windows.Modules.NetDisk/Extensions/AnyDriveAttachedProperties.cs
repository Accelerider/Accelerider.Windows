using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class AnyDriveAttachedProperties : BindableBase
    {
        private readonly IAcceleriderUser _user;

        public AnyDriveAttachedProperties(IAcceleriderUser user)
        {
            _user = user;
        }

        private INetDiskUser _currentNetDiskUser;
        private ObservableCollection<INetDiskUser> _netDiskUsers;

        public INetDiskUser CurrentNetDiskUser
        {
            get => _currentNetDiskUser;
            set => SetProperty(ref _currentNetDiskUser, value);
        }

        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get => _netDiskUsers;
            set => SetProperty(ref _netDiskUsers, value);
        }
    }
}
