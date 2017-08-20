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

        private ICommand _refreshFilesCommand;
        private bool _isLoadingFiles;
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

        public bool IsLoadingFiles
        {
            get => _isLoadingFiles;
            set => SetProperty(ref _isLoadingFiles, value);
        }

        public ObservableCollection<T> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }


        public override async void OnLoaded()
        {
            EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Subscribe(OnCurrentNetDiskUserChanged);

            if (PreviousNetDiskUser != NetDiskUser)
            {
                await LoadingFilesAsync();
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
            IsLoadingFiles = true;
            Files = new ObservableCollection<T>(await GetFilesAsync());
            IsLoadingFiles = false;
        }

        protected abstract Task<IEnumerable<T>> GetFilesAsync();
    }
}
