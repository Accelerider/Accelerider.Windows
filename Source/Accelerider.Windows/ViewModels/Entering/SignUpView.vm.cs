using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Models;
using Accelerider.Windows.Views.Entering;
using Microsoft.Practices.Unity;
using Refit;

namespace Accelerider.Windows.ViewModels.Entering
{
    public class SignUpViewModel : ViewModelBase
    {
        private string _emailAddress;
        private string _username;
        private string _licenseCode;

        private ICommand _signUpCommand;


        public SignUpViewModel(IUnityContainer container) : base(container)
        {
            SignUpCommand = new RelayCommand<SignUpView>(SignUpCommandExecute, SignUpCommandCanExecute);
        }

        public string EmailAddress
        {
            get => _emailAddress;
            set => SetProperty(ref _emailAddress, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string LicenseCode
        {
            get => _licenseCode;
            set => SetProperty(ref _licenseCode, value);
        }

        public ICommand SignUpCommand
        {
            get => _signUpCommand;
            set => SetProperty(ref _signUpCommand, value);
        }

        private bool SignUpCommandCanExecute(SignUpView view) => new[]
        {
            EmailAddress,
            Username,
            view.PasswordBox.Password,
            view.PasswordBoxRepeat.Password,
            LicenseCode
        }.All(field => !string.IsNullOrEmpty(field));

        private async void SignUpCommandExecute(SignUpView view)
        {
            if (view.PasswordBox.Password != view.PasswordBoxRepeat.Password)
            {
                GlobalMessageQueue.Enqueue("Password does not match the confirm password.");
                return;
            }
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);

            await SignUpAsync(new SignUpInfoBody
            {
                Username = Username,
                Password = view.PasswordBox.Password.ToMd5().EncryptByRsa()
            });

            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
        }

        private async Task SignUpAsync(SignUpInfoBody signUpInfo)
        {
            var nonAuthApi = Container.Resolve<INonAuthenticationApi>();
            await nonAuthApi.SignUpAsync(signUpInfo).RunApi(() =>
            {
                GlobalMessageQueue.Enqueue("Registered successfully!");
            });
        }
    }
}
