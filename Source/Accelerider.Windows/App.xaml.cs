using System;
using Accelerider.Windows.Properties;
using System.Windows;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.ServerInteraction;
using Accelerider.Windows.Views.Authentication;
using MaterialDesignThemes.Wpf;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Refit;

namespace Accelerider.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ProcessController.CheckSingleton();

            base.OnStartup(e);
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory(new ViewModelResolver(() => Container).UseDefaultConfigure().ResolveViewModelForView);
        }

        public override void Initialize()
        {
            base.Initialize();
            Settings.Default.PropertyChanged += (sender, eventArgs) => Settings.Default.Save();
        }

        protected override void OnInitialized()
        {
            ApiExceptionResolverExtension.SetUnityContainer(Container);
            ConfigureApplicationEventHandlers();
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoggerFacade, Logger>();
            containerRegistry.RegisterInstance<ISnackbarMessageQueue>(new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
            containerRegistry.RegisterInstance(new ConfigureFile().Load());
            containerRegistry.RegisterInstance(RestService.For<INonAuthenticationApi>(AcceleriderUrls.ApiBaseAddress));
        }

        protected override Window CreateShell() => new AuthenticationWindow();

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<ILoggerFacade, Logger>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog { ModulePath = @".\Modules" };
        }

        private void ConfigureApplicationEventHandlers()
        {
            var handler = Container.Resolve<ExceptionHandler>();
            AppDomain.CurrentDomain.UnhandledException += handler.UnhandledExceptionHandler;
            Current.DispatcherUnhandledException += handler.DispatcherUnhandledExceptionHandler;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            (Container.Resolve<ILoggerFacade>() as IDisposable)?.Dispose();
            base.OnExit(e);
        }
    }
}