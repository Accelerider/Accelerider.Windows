using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Resources.I18N;
using Prism.Regions;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.ServerInteraction;
using MaterialDesignThemes.Wpf;
using Prism.Modularity;
using Unity;


namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware, INotificable
    {
        private readonly IModuleManager _moduleManager;
        private readonly IModuleCatalog _moduleCatalog;
        private readonly INonAuthenticationApi _nonAuthenticationApi;
        private bool _appStoreIsDisplayed;

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        public MainWindowViewModel(
            IUnityContainer container,
            IRegionManager regionManager,
            IModuleManager moduleManager,
            IModuleCatalog moduleCatalog,
            INonAuthenticationApi nonAuthenticationApi) : base(container)
        {
            _moduleManager = moduleManager;
            _moduleCatalog = moduleCatalog;
            _nonAuthenticationApi = nonAuthenticationApi;
            RegionManager = regionManager;
            FeedbackCommand = new RelayCommand(() => Process.Start(AcceleriderUrls.Issue));
        }

        public IRegionManager RegionManager { get; }

        public ICommand FeedbackCommand { get; }

        public bool AppStoreIsDisplayed
        {
            get => _appStoreIsDisplayed;
            set
            {
                if (_appStoreIsDisplayed) return;
                if (!SetProperty(ref _appStoreIsDisplayed, value)) return;

                var region = RegionManager.Regions[RegionNames.MainTabRegion];
                foreach (var activeView in region.ActiveViews)
                {
                    region.Deactivate(activeView);
                }
            }
        }

        public void OnLoaded()
        {
#pragma warning disable 4014
            InitializeAppsAsync();
#pragma warning restore 4014

            var region = RegionManager.Regions[RegionNames.MainTabRegion];
            region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
            if (!region.Views.Any()) AppStoreIsDisplayed = true;

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

        public void OnUnloaded() { }

        private async Task InitializeAppsAsync()
        {
            var appMetadataList = await MockAllApps(); // TODO: await _nonAuthenticationApi.GetAppMetadataList().RunApi();

            // TODO: AcceleriderUser.Apps
            MockApps()
                .Select(item => appMetadataList
                    .FirstOrDefault(app => app.ModuleName
                        .Equals(item, StringComparison.InvariantCultureIgnoreCase)))
                .Where(item => item != null)
                .Do(item => item.InitializationMode = InitializationMode.WhenAvailable)
                .ForEach(item => _moduleCatalog.AddModule(item));

            MockApps().ForEach(item => _moduleManager.LoadModule(item));
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;

            _appStoreIsDisplayed = false;
            RaisePropertyChanged(nameof(AppStoreIsDisplayed));
        }

        // ----------------------------------------------------------------------------
        private static List<string> MockApps() => new List<string> { "Accelerider.Windows.Modules.NetDisk" };

        private static Task<List<AppMetadata>> MockAllApps() => Task.FromResult(new List<AppMetadata>
        {
            new AppMetadata
            {
                Id = "g123g1g2g8geywqge8712eug",
                ModuleName = "Accelerider.Windows.Modules.NetDisk",
                Ref = "https://file.mrs4s.me/file/3898c738090be65fc336577605014534",
                ModuleType = "Accelerider.Windows.Modules.NetDisk.NetDiskModule, Accelerider.Windows.Modules.NetDisk, Version=0.0.1.0, Culture=neutral, PublicKeyToken=null",
                Version = Version.Parse("0.0.1"),
                InitializationMode = InitializationMode.WhenAvailable,
                DependsOn = new Collection<string>()
            }
        });
    }
}
