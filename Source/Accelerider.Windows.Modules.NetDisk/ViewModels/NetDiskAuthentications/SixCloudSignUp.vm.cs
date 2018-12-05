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
    internal class SixCloudSignUpViewModel : ViewModelBase
    {
        private string _username;
        private string _phoneNumber;
        private string _passCode;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string PassCode
        {
            get => _passCode;
            set => SetProperty(ref _passCode, value);
        }

        public ICommand SendPassCodeCommand { get; }

        public ICommand SignUpCommand { get; }


        public SixCloudSignUpViewModel(IUnityContainer container) : base(container)
        {
            SendPassCodeCommand = new RelayCommand(() => { });

            SignUpCommand = new RelayCommand<PasswordBox>(
                passwordBox =>
                {

                },
                passwordBox => !string.IsNullOrWhiteSpace(passwordBox.Password) && 
                               passwordBox.Password.Length >= 6 &&
                               !string.IsNullOrWhiteSpace(Username) &&
                               !string.IsNullOrWhiteSpace(PhoneNumber) &&
                               !string.IsNullOrWhiteSpace(PassCode));
        }
    }
}
