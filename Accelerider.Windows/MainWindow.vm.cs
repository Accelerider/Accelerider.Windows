using Accelerider.Windows.ViewModels;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            GlobalMessageQueue.Enqueue("Welcome to Accelerider!");
        }
    }
}
