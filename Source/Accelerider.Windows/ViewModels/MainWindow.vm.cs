using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Common;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Resources.I18N;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _feedbackCommand;


        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            FeedbackCommand = new RelayCommand(() => Process.Start(ConstStrings.IssueUrl));

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

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
