using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Resources.I18N;
using Accelerider.Windows.Views.AppStore;
using Microsoft.Practices.Unity;
using Prism.Regions;
using System.Linq;

namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _feedbackCommand;
        private bool _appStoreIsDisplayed;


        public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            RegionManager = regionManager;
            FeedbackCommand = new RelayCommand(() => Process.Start(ConstStrings.IssueUrl));
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

        public override void OnLoaded(object view)
        {
            var region = RegionManager.Regions[RegionNames.MainTabRegion];
            region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
            if (!region.Views.Any()) AppStoreIsDisplayed = true;

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;

            _appStoreIsDisplayed = false;
            RaisePropertyChanged(nameof(AppStoreIsDisplayed));
        }
    }
}
