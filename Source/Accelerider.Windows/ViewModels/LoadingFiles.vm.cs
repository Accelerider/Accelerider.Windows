using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.ViewModels
{
    public abstract class LoadingFilesViewModel<T> : ViewModelBase
    {
        protected INetDiskUser PreviousNetDiskUser;

        private bool _isLoadingFiles;
        private ICommand _refreshFilesCommand;
        private IEnumerable<T> _files;


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
            set { if (SetProperty(ref _isLoadingFiles, value)) EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Publish(_isLoadingFiles); }
        }

        public IEnumerable<T> Files
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
            if (IsLoadingFiles) return;

            IsLoadingFiles = true;
            Files = await GetFilesAsync();
            IsLoadingFiles = false;
        }

        protected abstract Task<IEnumerable<T>> GetFilesAsync();
    }
}
