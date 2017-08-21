using System.Windows.Input;
using Accelerider.Windows.ViewModels;
using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Controls;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Infrastructure;
using System.Threading.Tasks;
using Accelerider.Windows.Events;
using MaterialDesignThemes.Wpf;
using Accelerider.Windows.Views.Dialogs;

namespace Accelerider.Windows
{
    public class EnteringWindowViewModel : ViewModelBase
    {
        private bool _isLoading;


        public EnteringWindowViewModel(IUnityContainer container) : base(container)
        {
            EventAggregator.GetEvent<IsLoadingMainWindowEvent>().Subscribe(e => IsLoading = e);
        }


        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
    }
}
