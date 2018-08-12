using Accelerider.Windows.Infrastructure.Interfaces;
using Autofac;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class ViewModelBase : BindableBase
    {
        private IAcceleriderUser _acceleriderUser;

        protected ViewModelBase(IContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<IEventAggregator>();
            Logger = container.Resolve<ILoggerFacade>();
            GlobalMessageQueue = container.Resolve<ISnackbarMessageQueue>();
        }

        public ISnackbarMessageQueue GlobalMessageQueue { get; }

        protected IContainer Container { get; }

        protected IEventAggregator EventAggregator { get; }

        protected ILoggerFacade Logger { get; }

        protected IAcceleriderUser AcceleriderUser => _acceleriderUser ?? (_acceleriderUser = Container.Resolve<IAcceleriderUser>());

        public virtual void OnLoaded(object view)
        {
        }

        public virtual void OnUnloaded(object view)
        {
        }
    }
}
