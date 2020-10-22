using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Models;
using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public abstract class LoadingFilesBaseViewModel<T> : ViewModelBase, IViewLoadedAndUnloadedAware
    {
        private IList<T> _files;

        protected LoadingFilesBaseViewModel(IUnityContainer container) : base(container)
        {
            RefreshFilesCommand = new RelayCommandAsync(async () => await LoadingFilesAsync());
        }

        public RelayCommandAsync RefreshFilesCommand { get; }

        public IList<T> Files
        {
            get => _files;
            private set => SetProperty(ref _files, value);
        }

        protected INetDiskUser PreviousNetDiskUser { get; set; }

        public virtual void OnLoaded()
        {
            if (PreviousNetDiskUser != CurrentNetDiskUser)
            {
                OnCurrentNetDiskUserChanged(CurrentNetDiskUser);
            }
        }

        public virtual void OnUnloaded() => PreviousNetDiskUser = CurrentNetDiskUser;

        protected async Task LoadingFilesAsync(IList<T> files = null)
        {
            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Publish(true);

            if (files == null)
            {
                var task = GetFilesAsync();
                if (task != null) Files = await task;
            }
            else
            {
                Files = files;
            }

            EventAggregator.GetEvent<IsLoadingFilesChangedEvent>().Publish(false);
        }

        protected abstract Task<IList<T>> GetFilesAsync();

        protected override async void OnCurrentNetDiskUserChanged(INetDiskUser currentNetDiskUser)
        {
            await currentNetDiskUser.RefreshAsync();
            await LoadingFilesAsync();
        }
    }
}
