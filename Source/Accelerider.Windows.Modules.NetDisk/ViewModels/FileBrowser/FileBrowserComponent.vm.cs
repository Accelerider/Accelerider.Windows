using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.Modules.NetDisk.Views.NetDiskAuthentications;
using Unity;
using MaterialDesignThemes.Wpf;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class FileBrowserComponentViewModel : ViewModelBase, IViewLoadedAndUnloadedAware, INotificable
    {
        private bool _canSwitchUser;
        private IEnumerable<ILazyTreeNode<INetDiskFile>> _searchResults;
        private ILazyTreeNode<INetDiskFile> _selectedSearchResult;

        public FileBrowserComponentViewModel(IUnityContainer container) : base(container)
        {
            SubscribeEvents();

            AddNetDiskCommand = new RelayCommandAsync(AddNetDiskCommandExecute);
        }

        public async void OnLoaded()
        {
            await AcceleriderUser.UpdateAsync();
            RaisePropertyChanged(nameof(NetDiskUsers));
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

        public ObservableCollection<INetDiskUser> NetDiskUsers =>
            new ObservableCollection<INetDiskUser>(AcceleriderUser.GetNetDiskUsers());

        public ICommand AddNetDiskCommand { get; }


        private static async Task AddNetDiskCommandExecute()
        {
            await DialogHost.Show(new SelectNetDiskTypeDialog(), "RootDialog");
        }

        private void SubscribeEvents()
        {
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Subscribe(isLoadingFiles => CanSwitchUser = !isLoadingFiles);
            EventAggregator.GetEvent<SearchResultsChangedEvent>().Subscribe(searchResults => SearchResults = searchResults);
        }

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }
    }
}
