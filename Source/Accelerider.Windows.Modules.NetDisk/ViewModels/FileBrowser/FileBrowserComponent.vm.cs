using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Views.NetDiskAuthentications;
using Unity;
using MaterialDesignThemes.Wpf;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class FileBrowserComponentViewModel : ViewModelBase, IAwareViewLoadedAndUnloaded
    {
        private bool _canSwitchUser;
        private IEnumerable<ILazyTreeNode<INetDiskFile>> _searchResults;
        private ILazyTreeNode<INetDiskFile> _selectedSearchResult;
        private ObservableCollection<INetDiskUser> _netDiskUsers;
        private ICommand _addNetDiskCommand;


        public FileBrowserComponentViewModel(IUnityContainer container) : base(container)
        {
            SubscribeEvents();

            AddNetDiskCommand = new RelayCommand(AddNetDiskCommandExecute);
        }

        public async void OnLoaded()
        {
            await AcceleriderUser.RefreshAsyncEx();
            NetDiskUsers = new ObservableCollection<INetDiskUser>(AcceleriderUser.GetNetDiskUsers());
        }

        public void OnUnloaded()
        {
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
                Debug.WriteLine($"SelectedSearchResult {value.Content.Path.FileName}");
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
            await DialogHost.Show(new SixCloud(), "RootDialog");
        }

        private void SubscribeEvents()
        {
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Subscribe(isLoadingFiles => CanSwitchUser = !isLoadingFiles);
            EventAggregator.GetEvent<SearchResultsChangedEvent>().Subscribe(searchResults => SearchResults = searchResults);
        }
    }
}
