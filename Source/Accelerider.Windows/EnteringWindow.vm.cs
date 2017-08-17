using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.ViewModels;
using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Controls;
using Accelerider.Windows.Commands;

namespace Accelerider.Windows
{
    public class EnteringWindowViewModel : ViewModelBase
    {
        private ICommand _signInCommand;


        public EnteringWindowViewModel(IUnityContainer container) : base(container)
        {
            SignInCommand = new RelayCommand<PasswordBox>(SignInCommandExecute);
        }

        public ICommand SignInCommand
        {
            get => _signInCommand;
            set => SetProperty(ref _signInCommand, value);
        }

        private void SignInCommandExecute(PasswordBox password)
        {
            new MainWindow().Show();
            (Application.Current.Resources[EnteringWindow.Key] as EnteringWindow)?.Close();
        }
    }
}
