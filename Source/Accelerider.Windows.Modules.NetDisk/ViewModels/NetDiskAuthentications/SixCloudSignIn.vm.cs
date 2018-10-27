using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.NetDiskAuthentications
{
    internal class SixCloudSignInViewModel : ViewModelBase
    {

        private string _username;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }


        public ICommand SignInCommand { get; }


        public SixCloudSignInViewModel(IUnityContainer container) : base(container)
        {
            SignInCommand = new RelayCommand<PasswordBox>(
                passwordBox =>
                {

                },
                passwordBox => !string.IsNullOrWhiteSpace(passwordBox.Password) &&
                               !string.IsNullOrWhiteSpace(Username));
        }
    }
}
