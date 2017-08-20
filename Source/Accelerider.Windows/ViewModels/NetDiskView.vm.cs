using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskViewModel : ViewModelBase
    {
        private ObservableCollection<INetDiskUser> _netDiskUsers;


        public NetDiskViewModel(IUnityContainer container) : base(container)
        {
            NetDiskUsers = new ObservableCollection<INetDiskUser>(AcceleriderUser.NetDiskUsers);
        }

        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get => _netDiskUsers;
            set => SetProperty(ref _netDiskUsers, value);
        }
    }
}
