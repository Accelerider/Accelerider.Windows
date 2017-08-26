using System.Collections.ObjectModel;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Events;
using System;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskViewModel : ViewModelBase
    {
        private bool _canSwitchUser;
        private ObservableCollection<INetDiskUser> _netDiskUsers;
        private ICommand _addNetDiskCommand;


        public NetDiskViewModel(IUnityContainer container) : base(container)
        {
            NetDiskUsers = new ObservableCollection<INetDiskUser>(AcceleriderUser.NetDiskUsers);
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Subscribe(isloadingFiles => CanSwitchUser = !isloadingFiles);

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
