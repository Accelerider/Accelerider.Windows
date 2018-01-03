using System;
using System.Windows;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using System.Net;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Views.Entering;
using Prism.Mvvm;
using Prism.Unity;
using Prism.Modularity;

namespace Accelerider.Windows
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Overridered methods
        //protected override IModuleCatalog CreateModuleCatalog() => new DirectoryModuleCatalog { ModulePath = @".\Modules" };
        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = "GroupModule",
                ModuleType = "Accelerider.Windows.Modules.Group.GroupModule, Accelerider.Windows.Modules.Group, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                Ref = $"file://{Environment.CurrentDirectory}/Modules/Accelerider.Windows.Modules.Group.dll",
                InitializationMode = InitializationMode.WhenAvailable
            });
            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = "NetDiskModule",
                ModuleType = "Accelerider.Windows.Modules.NetDisk.NetDiskModule, Accelerider.Windows.Modules.NetDisk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                Ref = $"file://{Environment.CurrentDirectory}/Modules/Accelerider.Windows.Modules.NetDisk.dll",
                InitializationMode = InitializationMode.WhenAvailable
            });
        }

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
