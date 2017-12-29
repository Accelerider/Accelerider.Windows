using System.Linq;

using Accelerider.Windows.Common;
using Accelerider.Windows.Infrastructure.Interfaces;

using MaterialDesignThemes.Wpf;

using Microsoft.Practices.Unity;

using Prism.Events;
using Prism.Mvvm;

namespace Accelerider.Windows.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        private SnackbarMessageQueue _globalMessageQueue;

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<IEventAggregator>();
            AcceleriderUser = container.Resolve<IAcceleriderUser>();
            GlobalMessageQueue = container.Resolve<SnackbarMessageQueue>();
        }

        public INetDiskUser NetDiskUser
        {
            get => AcceleriderUser.CurrentNetDiskUser ?? AcceleriderUser.NetDiskUsers.FirstOrDefault();
            set
            {
                var temp = AcceleriderUser.CurrentNetDiskUser;
                if (!SetProperty(ref temp, value)) return;
                AcceleriderUser.CurrentNetDiskUser = temp;
                EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Publish(temp);
            }
        }

        public SnackbarMessageQueue GlobalMessageQueue
        {
            get => _globalMessageQueue;
            set => SetProperty(ref _globalMessageQueue, value);
        }

        protected IUnityContainer Container { get; }
        protected IEventAggregator EventAggregator { get; }
        protected IAcceleriderUser AcceleriderUser { get; }

        public virtual void OnLoaded(object view)
        {
        }

        public virtual void OnUnloaded(object view)
        {
        }
    }
}
