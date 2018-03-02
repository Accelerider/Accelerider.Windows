using System;
using System.Collections.Generic;
using System.Windows;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using System.Net;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Views.Entering;
using Prism.Mvvm;
using Prism.Unity;
using Prism.Logging;

namespace Accelerider.Windows
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Overridered methods
        protected override ILoggerFacade CreateLogger() => new Logger();

        //protected override void ConfigureModuleCatalog()
        //{
        //    new ModuleResolver(ModuleCatalog).Initialize();
        //}

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.Resolve<Core.Module>().Initialize();
            Container.RegisterInstance(new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
        }

        protected override void ConfigureViewModelLocator() => ViewModelLocationProvider.SetDefaultViewModelFactory(ResolveViewModel);

        protected override DependencyObject CreateShell() => new EnteringWindow();

        protected override void InitializeShell()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
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
            Container.Resolve<IAcceleriderUser>().OnExit();
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
