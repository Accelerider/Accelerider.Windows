using System;
using System.Windows;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using System.Net;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Models;
using Accelerider.Windows.Views.Entering;
using Prism.Mvvm;
using Prism.Unity;
using Prism.Logging;
using Refit;

namespace Accelerider.Windows
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Overridered methods
        protected override ILoggerFacade CreateLogger() => new Logger();

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            //Container.Resolve<Core.Module>().Initialize(); // TODO: [Obsolete] TO DELETE.
            //Container.RegisterType<ModuleResolver, ModuleResolver>(new ContainerControlledLifetimeManager());
            Container.RegisterInstance(typeof(ISnackbarMessageQueue), new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
            Container.RegisterInstance(new ConfigureFile().Load());
            Container.RegisterInstance(RestService.For<INonAuthenticationApi>(ConstStrings.BaseAddress));
        }

        protected override void ConfigureViewModelLocator() => ViewModelLocationProvider.SetDefaultViewModelFactory(ResolveViewModel);

        protected override DependencyObject CreateShell() => new EnteringWindow();

        protected override void InitializeShell()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ApiExceptionResolverExtension.SetUnityContainer(Container);
            ConfigureApplicationEventHandlers();
            ShellSwitcher.Show((Window)Shell);
        }
        #endregion

        #region Private methods
        private void ConfigureApplicationEventHandlers()
        {
            var resolver = Container.Resolve<ExceptionResolver>();
            AppDomain.CurrentDomain.UnhandledException += resolver.UnhandledExceptionHandler;
            Application.Current.DispatcherUnhandledException += resolver.DispatcherUnhandledExceptionHandler;

            Application.Current.Exit += OnExit;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            //Container.Resolve<IAcceleriderUser>().OnExit();
            (Logger as IDisposable)?.Dispose();
        }

        private object ResolveViewModel(object view, Type viewModelType)
        {
            var viewModel = Container.Resolve(viewModelType);
            if (view is FrameworkElement frameworkElement &&
                viewModel is ViewModelBase viewModelBase)
            {
                frameworkElement.Loaded += (sender, e) => viewModelBase.OnLoaded(sender);
                frameworkElement.Unloaded += (sender, e) => viewModelBase.OnUnloaded(sender);
            }
            return viewModel;
        }
        #endregion
    }
}
