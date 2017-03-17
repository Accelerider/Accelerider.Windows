using System.Windows.Controls;
using System.Windows.Input;
using BaiduPanDownloadWpf.Commands;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal abstract class ViewModelBase : BindableBase
    {
        private ICommand _loadedCommand;
        private ICommand _loadedCommandWithParam;

        public ICommand LoadedCommand
        {
            get { return _loadedCommand; }
            set { SetProperty(ref _loadedCommand, value); }
        }
        public ICommand LoadedCommandWithParam
        {
            get { return _loadedCommandWithParam; }
            set { SetProperty(ref _loadedCommandWithParam, value); }
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
            LoadedCommandWithParam = new Command<ContentControl>(OnLoaded);
        }

        protected virtual void OnLoaded() { }
        protected virtual void OnLoaded(ContentControl param) { }
    }
}
