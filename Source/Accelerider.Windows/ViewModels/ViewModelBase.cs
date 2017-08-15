using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        private INetDiskUser _netDiskUser;
        private SnackbarMessageQueue _globalMessageQueue;


        protected IUnityContainer Container { get; }
        protected EventAggregator EventAggregator { get; }
        protected IAcceleriderUser AcceleriderUser { get; }
        public INetDiskUser NetDiskUser
        {
            get => _netDiskUser;
            set
            {
                if (SetProperty(ref _netDiskUser, value))
                {
                    AcceleriderUser.CurrentNetDiskUser = value;
                    OnCurrentNetDiskUserChanged();
                }
            }
        }
        public SnackbarMessageQueue GlobalMessageQueue
        {
            get => _globalMessageQueue;
            set => SetProperty(ref _globalMessageQueue, value);
        }


        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<EventAggregator>();
            AcceleriderUser = container.Resolve<IAcceleriderUser>();
            NetDiskUser = AcceleriderUser.CurrentNetDiskUser;
            GlobalMessageQueue = container.Resolve<SnackbarMessageQueue>();
        }


        public virtual void OnLoadAsync() { }

        protected virtual void OnCurrentNetDiskUserChanged() { }
    }
}
