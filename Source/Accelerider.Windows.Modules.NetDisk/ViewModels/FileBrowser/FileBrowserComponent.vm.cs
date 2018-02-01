using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private IEnumerable<INetDiskFile> _searchResults;
        private INetDiskFile _selectedSearchResult;
        private ObservableCollection<INetDiskUser> _netDiskUsers;
        private ICommand _addNetDiskCommand;


        public FileBrowserComponentViewModel(IUnityContainer container) : base(container)
        {
            NetDiskUsers = new ObservableCollection<INetDiskUser>(AcceleriderUser.NetDiskUsers);

            SubscrubeEvents();

            AddNetDiskCommand = new RelayCommand(AddNetDiskCommandExecute);
        }


        public bool CanSwitchUser
        {
            get => _canSwitchUser;
            set => SetProperty(ref _canSwitchUser, value);
        }

        public IEnumerable<INetDiskFile> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public INetDiskFile SelectedSearchResult
        {
            get => _selectedSearchResult;
            set
            {
                if (SetProperty(ref _selectedSearchResult, value))
                {
                    EventAggregator.GetEvent<SelectedSearchResultChangedEvent>().Publish(value);
                    Debug.WriteLine("SelectedSearchResult Updated!");
                }
            }
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

        private void SubscrubeEvents()
        {
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Subscribe(isLoadingFiles => CanSwitchUser = !isLoadingFiles);
            EventAggregator.GetEvent<SearchResultsChangedEvent>().Subscribe(searchResults => SearchResults = searchResults);
        }
    }
}
