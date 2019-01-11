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
using Prism.Unity;
using Refit;

namespace Accelerider.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string ProcessName = @"Global\Accelerider.Windows.Wpf";

        protected override void OnStartup(StartupEventArgs e)
        {
            ProcessController.CheckSingleton(ProcessName, (IntPtr)Settings.Default.WindowHandle);
            ConfigureApplicationEventHandlers();

            base.OnStartup(e);
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory(new ViewModelResolver(() => Container)
                .UseDefaultConfigure()
                .ResolveViewModelForView);
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeCultureInfo();
            Settings.Default.PropertyChanged += (sender, eventArgs) => Settings.Default.Save();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<ILoggerFacade, Log4NetLogger>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ISnackbarMessageQueue>(new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
            containerRegistry.RegisterInstance(RestService.For<INonAuthenticationApi>(AcceleriderUrls.ApiBaseAddress));
            containerRegistry.RegisterSingleton<IUpgradeService, UpgradeService>();
            containerRegistry.RegisterInstance<IDataRepository>(new DataRepository(
                () => Container.Resolve<IAcceleriderUser>().Email));
        }

        protected override Window CreateShell() => null;

        protected override void OnInitialized()
        {
            ApiExceptionResolverExtension.SetUnityContainer(Container);
            WindowHelper.Container = Container.GetContainer();

            WindowHelper.SwitchTo<AuthenticationWindow>();
        }

        // ---------------------------------------------------------------------------------------------------------------

        private static void ConfigureApplicationEventHandlers()
        {
            var handler = new ExceptionHandler();
            AppDomain.CurrentDomain.UnhandledException += handler.UnhandledExceptionHandler;
            Current.DispatcherUnhandledException += handler.DispatcherUnhandledExceptionHandler;
        }

        private void InitializeCultureInfo()
        {
            var settings = Container.Resolve<IDataRepository>().Get<ShellSettings>(isGlobal: true);

            I18nManager.Initialize(settings);

            if (settings.Language == null)
            {
                settings.Language = CultureInfo.InstalledUICulture;
            }

            I18nManager.Instance.CurrentUICulture = settings.Language;
            I18nManager.Instance.AddResourceManager(UiResources.ResourceManager);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<IUpgradeService>().Stop();

            ProcessController.Clear();

            Container.Resolve<IEventAggregator>().GetEvent<ApplicationExiting>().Publish();

            (Container.Resolve<ILoggerFacade>() as IDisposable)?.Dispose();
            base.OnExit(e);
        }
    }
}