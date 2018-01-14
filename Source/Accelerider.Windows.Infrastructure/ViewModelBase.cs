using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class ViewModelBase : BindableBase
    {
        private ISnackbarMessageQueue _globalMessageQueue;

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<IEventAggregator>();
            Logger = container.Resolve<ILoggerFacade>();
            AcceleriderUser = container.Resolve<IAcceleriderUser>();
            GlobalMessageQueue = container.Resolve<ISnackbarMessageQueue>();
        }

        public ISnackbarMessageQueue GlobalMessageQueue
        {
            get => _globalMessageQueue;
            set => SetProperty(ref _globalMessageQueue, value);
        }

        protected IUnityContainer Container { get; }
        protected IEventAggregator EventAggregator { get; }
        protected ILoggerFacade Logger { get; }

        protected IAcceleriderUser AcceleriderUser { get; }

        public virtual void OnLoaded(object view)
        {
        }

        public virtual void OnUnloaded(object view)
        {
        }
    }
}
