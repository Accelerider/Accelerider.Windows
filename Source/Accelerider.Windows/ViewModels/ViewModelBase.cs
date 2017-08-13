using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        private SnackbarMessageQueue _globalMessageQueue;
        private bool _isViewModelLoaded;

        protected IUnityContainer Container { get; }
        protected EventAggregator EventAggregator { get; }
        protected IAcceleriderUser AcceleriderUser { get; }
        protected INetDiskUser NetDiskUser { get; set; }

        public SnackbarMessageQueue GlobalMessageQueue
        {
            get => _globalMessageQueue;
            set => SetProperty(ref _globalMessageQueue, value);
        }
        public bool IsViewModelLoaded
        {
            get => _isViewModelLoaded;
            set => SetProperty(ref _isViewModelLoaded, value);
        }

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<EventAggregator>();
            AcceleriderUser = container.Resolve<IAcceleriderUser>();
            NetDiskUser = AcceleriderUser.CurrentNetDiskUser;
            GlobalMessageQueue = container.Resolve<SnackbarMessageQueue>();
        }

        public async Task OnViewLoaded()
        {
            IsViewModelLoaded = false;
            await Load();
            IsViewModelLoaded = true;
        }

        protected virtual async Task Load()
        {
            NetDiskUser = AcceleriderUser.CurrentNetDiskUser;
        }
    }
}
