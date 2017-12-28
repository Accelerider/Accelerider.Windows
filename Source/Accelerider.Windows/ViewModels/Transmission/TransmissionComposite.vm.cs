using Accelerider.Windows.Commands;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private async void AddOfflineTaskCommandExecute()
        {
            await DialogHost.Show(new AddOfflineTaskDialog(), "RootDialog");
        }

    }
}
