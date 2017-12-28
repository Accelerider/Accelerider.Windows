using System.Windows.Input;

using Accelerider.Windows.Commands;
using Accelerider.Windows.Views.Dialogs;

using MaterialDesignThemes.Wpf;

using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Transmission
{
    public class TransmissionCompositeViewModel : ViewModelBase
    {
        private ICommand _addOfflineTaskCommand;

        public TransmissionCompositeViewModel(IUnityContainer container) : base(container)
        {
            AddOfflineTaskCommand = new RelayCommand(AddOfflineTaskCommandExecute);
        }

        public ICommand AddOfflineTaskCommand
        {
            get => _addOfflineTaskCommand;
            set => SetProperty(ref _addOfflineTaskCommand, value);
        }

        private async void AddOfflineTaskCommandExecute() => await DialogHost.Show(new AddOfflineTaskDialog(), "RootDialog");
    }
}
