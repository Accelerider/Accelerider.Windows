using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        private SnackbarMessageQueue _globalMessageQueue;
        private bool _isViewModelLoaded;

        protected IUnityContainer Container { get; }
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
            AcceleriderUser = container.Resolve<IAcceleriderUser>();
            NetDiskUser = AcceleriderUser.CurrentNetDiskUser;
            GlobalMessageQueue = container.Resolve<SnackbarMessageQueue>();
        }

        public async Task OnViewLoaded()
        {
            IsViewModelLoaded = false;
            await LoadViewModel();
            IsViewModelLoaded = true;
        }

        protected virtual async Task LoadViewModel()
        {
            NetDiskUser = AcceleriderUser.CurrentNetDiskUser;
        }
    }
}
