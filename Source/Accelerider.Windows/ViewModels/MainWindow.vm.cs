using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Resources.I18N;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _feedbackCommand;


        public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            RegionManager = regionManager;
            FeedbackCommand = new RelayCommand(() => Process.Start(ConstStrings.IssueUrl));

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

        public IRegionManager RegionManager { get; }

        public ICommand FeedbackCommand
        {
            get => _feedbackCommand;
            set => SetProperty(ref _feedbackCommand, value);
        }

        public override void OnUnloaded(object view)
        {
            AcceleriderUser.OnExit();
        }
    }
}
