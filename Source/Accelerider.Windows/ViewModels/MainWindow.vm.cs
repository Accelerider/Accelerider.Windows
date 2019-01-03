using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Resources.I18N;
using Prism.Regions;
using System.Linq;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure.Mvvm;
using MaterialDesignThemes.Wpf;
using Unity;


namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware, INotificable
    {
        private ICommand _feedbackCommand;
        private bool _appStoreIsDisplayed;

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            RegionManager = regionManager;
            FeedbackCommand = new RelayCommand(() => Process.Start(AcceleriderUrls.Issue));
        }

        public IRegionManager RegionManager { get; }

        public ICommand FeedbackCommand
        {
            get => _feedbackCommand;
            set => SetProperty(ref _feedbackCommand, value);
        }

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
            var region = RegionManager.Regions[RegionNames.MainTabRegion];
            region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
            if (!region.Views.Any()) AppStoreIsDisplayed = true;

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

        public void OnUnloaded()
        {
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;

            _appStoreIsDisplayed = false;
            RaisePropertyChanged(nameof(AppStoreIsDisplayed));
        }
    }
}
