using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Events;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskViewModel : ViewModelBase
    {
        private bool _canSwitchUser;
        private ObservableCollection<INetDiskUser> _netDiskUsers;


        public NetDiskViewModel(IUnityContainer container) : base(container)
        {
            NetDiskUsers = new ObservableCollection<INetDiskUser>(AcceleriderUser.NetDiskUsers);
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Subscribe(isloadingFiles => CanSwitchUser = !isloadingFiles);
        }


        public bool CanSwitchUser
        {
            get { return _canSwitchUser; }
            set { SetProperty(ref _canSwitchUser, value); }
        }

        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get => _netDiskUsers;
            set => SetProperty(ref _netDiskUsers, value);
        }
    }
}
