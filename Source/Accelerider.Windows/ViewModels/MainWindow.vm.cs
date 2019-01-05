using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Resources.I18N;
using Prism.Regions;
using System.Linq;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Infrastructure.Upgrade;
using MaterialDesignThemes.Wpf;
using Unity;
using Unity.Resolution;


namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware, INotificable
    {
        private bool _appStoreIsDisplayed;

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
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
            RunUpgradeService();

            var region = RegionManager.Regions[RegionNames.MainTabRegion];
            region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
            if (!region.Views.Any()) AppStoreIsDisplayed = true;

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

        public void OnUnloaded() { }

        private void RunUpgradeService()
        {
            // TODO: Delete Mock
            AcceleriderUser.Apps = new List<string> { "apps-net-disk-win" };

            var upgradeService = Container.Resolve<IUpgradeService>();

            upgradeService.Add(Container.Resolve<ShellUpgradeTask>());
            upgradeService.AddRange(AcceleriderUser.Apps.Select(item =>
                Container.Resolve<AppUpgradeTask>(new ParameterOverride(AppUpgradeTask.ParameterCtorName, item))));

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
