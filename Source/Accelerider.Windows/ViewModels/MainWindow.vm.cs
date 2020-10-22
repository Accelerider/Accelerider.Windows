using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Resources.I18N;
using Prism.Regions;
using System.Linq;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Infrastructure.Upgrade;
using Accelerider.Windows.Upgrade;
using MaterialDesignThemes.Wpf;
using Unity;
using Unity.Resolution;
#if DEBUG
using Prism.Modularity;
#endif

namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware, INotificable
    {
        private readonly IRegionManager _regionManager;
        private bool _appStoreIsDisplayed;

        public ICommand FeedbackCommand { get; }

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            _regionManager = regionManager;
            FeedbackCommand = new RelayCommand(() => Process.Start(AcceleriderUrls.Issues));
        }

        public bool AppStoreIsDisplayed
        {
            get => _appStoreIsDisplayed;
            set
            {
                if (_appStoreIsDisplayed) return;
                if (!SetProperty(ref _appStoreIsDisplayed, value)) return;

                var region = _regionManager.Regions[RegionNames.MainTabRegion];
                foreach (var activeView in region.ActiveViews)
                {
                    region.Deactivate(activeView);
                }
            }
        }

        public void OnLoaded()
        {
#if DEBUG
            Container.Resolve<IModuleManager>().Run();
#else
            RunUpgradeService();
#endif

            var region = _regionManager.Regions[RegionNames.MainTabRegion];
            region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
            if (!region.Views.Any()) AppStoreIsDisplayed = true;

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

        public void OnUnloaded() { }

        private void RunUpgradeService()
        {
            // TODO: Delete Mock
            AcceleriderUser.Apps = new List<string> { "app-any-drive" };

            var upgradeService = Container.Resolve<IUpgradeService>();

            upgradeService.Add(Container.Resolve<ShellUpgradeTask>());
            upgradeService.AddRange(AcceleriderUser.Apps.Select(item =>
                Container.Resolve<AppModuleUpgradeTask>(new ParameterOverride(AppModuleUpgradeTask.ParameterCtorName, item))));

            upgradeService.Run();
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;

            _appStoreIsDisplayed = false;
            RaisePropertyChanged(nameof(AppStoreIsDisplayed));
        }
    }
}
