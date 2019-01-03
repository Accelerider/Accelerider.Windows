using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Views.Dialogs;
using Unity;
using MaterialDesignThemes.Wpf;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class TransportationComponentViewModel : ViewModelBase
    {
        private ICommand _addOfflineTaskCommand;

        public TransportationComponentViewModel(IUnityContainer container) : base(container)
        {
            AddOfflineTaskCommand = new RelayCommand(AddOfflineTaskCommandExecute);

            ConfigureTransferList();
        }

        public ICommand AddOfflineTaskCommand
        {
            get => _addOfflineTaskCommand;
            set => SetProperty(ref _addOfflineTaskCommand, value);
        }

        private async void AddOfflineTaskCommandExecute() =>
            await DialogHost.Show(new AddOfflineTaskDialog(), "RootDialog");

        private void ConfigureTransferList() { }
    }
}
