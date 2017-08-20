using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public abstract class LoadingFilesViewModel<T> : ViewModelBase
    {
        protected INetDiskUser PreviousNetDiskUser;
        private int _loadingCount;

        private ICommand _refreshFilesCommand;
        private ObservableCollection<T> _files;


        protected LoadingFilesViewModel(IUnityContainer container) : base(container)
        {
            RefreshFilesCommand = new RelayCommand(async () => await LoadingFilesAsync());
        }


        public ICommand RefreshFilesCommand
        {
            get => _refreshFilesCommand;
            set => SetProperty(ref _refreshFilesCommand, value);
        }

        public int LoadingCount
        {
            get => _loadingCount;
            set
            {
                if (_loadingCount == value) return;
                _loadingCount = value;
                OnPropertyChanged(nameof(IsLoadingFiles));
            }
        }

        public bool IsLoadingFiles => _loadingCount != 0;

        public ObservableCollection<T> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }


        public override void OnLoaded()
        {
            EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Subscribe(OnCurrentNetDiskUserChanged);

            if (PreviousNetDiskUser != NetDiskUser)
            {
                OnCurrentNetDiskUserChanged(NetDiskUser);
            }
        }

        public override void OnUnloaded()
        {
            EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Unsubscribe(OnCurrentNetDiskUserChanged);

            PreviousNetDiskUser = NetDiskUser;
        }

        private async void OnCurrentNetDiskUserChanged(INetDiskUser currentNetDiskUser)
        {
            await LoadingFilesAsync();
        }

        protected async Task LoadingFilesAsync()
        {
            LoadingCount++;
            var temp = await GetFilesAsync();
            if (temp != null) Files = new ObservableCollection<T>(temp);
            LoadingCount--;
        }

        protected abstract Task<IEnumerable<T>> GetFilesAsync();
    }
}
