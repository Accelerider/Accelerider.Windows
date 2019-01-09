using System;
using System.Globalization;
using Accelerider.Windows.Properties;
using System.Windows;
using Accelerider.Windows.Constants;
using Accelerider.Windows.I18nResources;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.I18n;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Infrastructure.Upgrade;
using Accelerider.Windows.ServerInteraction;
using Accelerider.Windows.Views.Authentication;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using Prism.Ioc;
using Prism.Logging;
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
            ViewModelLocationProvider.SetDefaultViewModelFactory(new ViewModelResolver(() => Container)
                .UseDefaultConfigure()
                .ResolveViewModelForView);
        }

        protected override Window CreateShell()
        {
            InitializeCultureInfo();
            return new AuthenticationWindow();
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
            containerRegistry.RegisterInstance<ISnackbarMessageQueue>(new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
            containerRegistry.RegisterInstance(new ConfigureFile().Load());
            containerRegistry.RegisterInstance(RestService.For<INonAuthenticationApi>(AcceleriderUrls.ApiBaseAddress));
            containerRegistry.RegisterSingleton<IUpgradeService, UpgradeService>();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            //containerRegistry.RegisterSingleton<ILoggerFacade, Logger>();
            containerRegistry.RegisterSingleton<ILoggerFacade, Log4NetLogger>();
        }

        private void ConfigureApplicationEventHandlers()
        {
            var handler = Container.Resolve<ExceptionHandler>();
            AppDomain.CurrentDomain.UnhandledException += handler.UnhandledExceptionHandler;
            Current.DispatcherUnhandledException += handler.DispatcherUnhandledExceptionHandler;
        }

        private void InitializeCultureInfo()
        {
            var configure = Container.Resolve<IConfigureFile>();

            I18nManager.Initialize(configure);
            var language = configure.GetValue<CultureInfo>(ConfigureKeys.Language);
            if (language == null)
            {
                language = CultureInfo.InstalledUICulture;
                configure.SetValue(ConfigureKeys.Language, language);
            }

            I18nManager.Instance.CurrentUICulture = language;
            I18nManager.Instance.AddResourceManager(UiResources.ResourceManager);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<IEventAggregator>().GetEvent<ApplicationExiting>().Publish();

            (Container.Resolve<ILoggerFacade>() as IDisposable)?.Dispose();
            base.OnExit(e);
        }
    }
}