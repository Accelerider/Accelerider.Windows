using System.Linq;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        private SnackbarMessageQueue _globalMessageQueue;


        protected IUnityContainer Container { get; }
        protected EventAggregator EventAggregator { get; }
        protected IAcceleriderUser AcceleriderUser { get; }

        public INetDiskUser NetDiskUser
        {
            get => AcceleriderUser.CurrentNetDiskUser ?? AcceleriderUser.NetDiskUsers.FirstOrDefault();
            set
            {
                var temp = AcceleriderUser.CurrentNetDiskUser;
                if (SetProperty(ref temp, value))
                {
                    AcceleriderUser.CurrentNetDiskUser = temp;
                    EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Publish(temp);
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
            GlobalMessageQueue = container.Resolve<SnackbarMessageQueue>();
        }

        public virtual void OnLoaded(object view) { }

        public virtual void OnUnloaded(object view) { }
    }
}
