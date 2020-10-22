using System;
using System.Windows.Threading;
using Prism.Events;
using Prism.Mvvm;
using Unity;

namespace Accelerider.Windows.Infrastructure.Mvvm
{
    public abstract class ViewModelBase : BindableBase
    {
        private IAcceleriderUser _acceleriderUser;

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<IEventAggregator>();
        }

        public Dispatcher Dispatcher { get; set; }

        protected IUnityContainer Container { get; }

        protected IEventAggregator EventAggregator { get; }

        public IAcceleriderUser AcceleriderUser => _acceleriderUser ?? (_acceleriderUser = Container.Resolve<IAcceleriderUser>());

        // ReSharper disable once InconsistentNaming
        protected virtual void InvokeOnUIThread(Action action) => Dispatcher.Invoke(action);
    }
}
