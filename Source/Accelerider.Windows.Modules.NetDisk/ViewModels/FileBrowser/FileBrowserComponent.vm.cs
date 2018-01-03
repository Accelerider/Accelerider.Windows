using System.Collections.ObjectModel;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class FileBrowserComponentViewModel : ViewModelBase
    {
        private bool _canSwitchUser;
        private ObservableCollection<INetDiskUser> _netDiskUsers;
        private ICommand _addNetDiskCommand;

        public FileBrowserComponentViewModel(IUnityContainer container) : base(container)
        {
            NetDiskUsers = new ObservableCollection<INetDiskUser>(AcceleriderUser.NetDiskUsers);
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Subscribe(isLoadingFiles => CanSwitchUser = !isLoadingFiles);

            AddNetDiskCommand = new RelayCommand(AddNetDiskCommandExecute);
        }

        public bool CanSwitchUser
        {
            get => _canSwitchUser;
            set => SetProperty(ref _canSwitchUser, value);
        }

        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get => _netDiskUsers;
            set => SetProperty(ref _netDiskUsers, value);
        }

        public ICommand AddNetDiskCommand
        {
            get => _addNetDiskCommand;
            set => SetProperty(ref _addNetDiskCommand, value);
        }

        private async void AddNetDiskCommandExecute()
        {
            await DialogHost.Show(new SelectNetDiskTypeDialog(), "RootDialog");
        }
    }
}
