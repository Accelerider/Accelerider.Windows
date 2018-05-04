using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class FileBrowserComponentViewModel : ViewModelBase
    {
        private bool _canSwitchUser;
        private IEnumerable<ILazyTreeNode<INetDiskFile>> _searchResults;
        private ILazyTreeNode<INetDiskFile> _selectedSearchResult;
        private ObservableCollection<INetDiskUser> _netDiskUsers;
        private ICommand _addNetDiskCommand;


        public FileBrowserComponentViewModel(IUnityContainer container) : base(container)
        {
            NetDiskUsers = new ObservableCollection<INetDiskUser>(AcceleriderUser.GetNetDiskUsers());

            SubscribeEvents();

            AddNetDiskCommand = new RelayCommand(AddNetDiskCommandExecute);
        }


        public bool CanSwitchUser
        {
            get => _canSwitchUser;
            set => SetProperty(ref _canSwitchUser, value);
        }

        public IEnumerable<ILazyTreeNode<INetDiskFile>> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public ILazyTreeNode<INetDiskFile> SelectedSearchResult
        {
            get => _selectedSearchResult;
            set
            {
                if (!SetProperty(ref _selectedSearchResult, value) || value == null) return;

                EventAggregator.GetEvent<SelectedSearchResultChangedEvent>().Publish(value);

#if DEBUG
                Debug.WriteLine($"SelectedSearchResult {value.Content.FilePath.FileName}");
#endif
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

        private void SubscribeEvents()
        {
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Subscribe(isLoadingFiles => CanSwitchUser = !isLoadingFiles);
            EventAggregator.GetEvent<SearchResultsChangedEvent>().Subscribe(searchResults => SearchResults = searchResults);
        }
    }
}
