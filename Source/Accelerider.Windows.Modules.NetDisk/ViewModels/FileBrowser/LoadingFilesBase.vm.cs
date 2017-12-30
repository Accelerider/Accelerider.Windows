using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public abstract class LoadingFilesBaseViewModel<T> : ViewModelBase
    {
        private bool _isLoadingFiles;
        private ICommand _refreshFilesCommand;
        private IEnumerable<T> _files;

        protected LoadingFilesBaseViewModel(IUnityContainer container) : base(container)
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
            set
            {
                SetProperty(ref _files, value);
                IsLoadingFiles = false;
            }
        }

        protected INetDiskUser PreviousNetDiskUser { get; set; }

        public override void OnLoaded(object view)
        {
            EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Subscribe(OnCurrentNetDiskUserChanged);

            if (PreviousNetDiskUser != NetDiskUser)
            {
                OnCurrentNetDiskUserChanged(NetDiskUser);
            }
        }

        public override void OnUnloaded(object view)
        {
            EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Unsubscribe(OnCurrentNetDiskUserChanged);

            PreviousNetDiskUser = NetDiskUser;
        }

        protected async Task LoadingFilesAsync()
        {
            if (IsLoadingFiles) return;

            IsLoadingFiles = true;
            var task = GetFilesAsync();
            if (task != null) Files = await task;
        }

        protected abstract Task<IEnumerable<T>> GetFilesAsync();

        private async void OnCurrentNetDiskUserChanged(INetDiskUser currentNetDiskUser)
        {
            await currentNetDiskUser.RefreshUserInfoAsync();
            await LoadingFilesAsync();
        }
    }
}
