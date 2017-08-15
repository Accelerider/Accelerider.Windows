using System.Threading.Tasks;
using Accelerider.Windows.ViewModels;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Assets;

namespace Accelerider.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);
        }

        //protected override async Task LoadViewModel()
        //{
        //    await DialogHost.Show(new EnteringDialog(), "RootDialog");
        //}
    }
}
