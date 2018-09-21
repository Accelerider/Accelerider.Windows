using System;
using System.Windows.Threading;
using Autofac;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure.ViewModels
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

        public Dispatcher Dispatcher { get; set; }

        protected IContainer Container { get; }

        protected IEventAggregator EventAggregator { get; }

        protected ILoggerFacade Logger { get; }

        public IAcceleriderUser AcceleriderUser => _acceleriderUser ?? (_acceleriderUser = Container.Resolve<IAcceleriderUser>());

        protected virtual void Invoke(Action action) => Dispatcher.Invoke(action);
    }
}
