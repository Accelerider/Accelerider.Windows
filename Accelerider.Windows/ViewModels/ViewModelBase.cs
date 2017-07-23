using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        protected IUnityContainer Container { get; }
        protected IAcceleriderUser AcceleriderUser { get; }
        protected INetDiskUser NetDiskUser { get; }


        private bool _isViewModelLoaded;
        public bool IsViewModelLoaded
        {
            get => _isViewModelLoaded;
            set => SetProperty(ref _isViewModelLoaded, value);
        }



        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            AcceleriderUser = container.Resolve<IAcceleriderUser>();
            NetDiskUser = container.Resolve<INetDiskUser>();
        }


        public async Task OnViewLoaded()
        {
            IsViewModelLoaded = false;
            await LoadViewModel();
            IsViewModelLoaded = true;
        }

        protected virtual async Task LoadViewModel() { }
    }
}
