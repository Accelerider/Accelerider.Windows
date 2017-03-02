using BaiduPanDownloadWpf.Commands;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal abstract class ViewModelBase : BindableBase
    {
        private Command _loadedCommand;
        public Command LoadedCommand
        {
            get { return _loadedCommand; }
            set { SetProperty(ref _loadedCommand, value); }
        }


        protected IUnityContainer Container { get; }
        protected IEventAggregator EventAggregator { get; }
        protected ILoggerFacade Logger { get; }

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<IEventAggregator>();
            Logger = container.Resolve<ILoggerFacade>();
            LoadedCommand = new Command(OnLoaded);
        }

        protected virtual void OnLoaded() { }
    }
}
