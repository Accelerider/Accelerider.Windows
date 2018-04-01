using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Extensions;
using Accelerider.Windows.Models;
using Accelerider.Windows.Views.Entering;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Entering
{
    public class SignUpViewModel : ViewModelBase
    {
        private string _emailAddress;
        private string _username;

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
        }.All(field => !string.IsNullOrEmpty(field));

        private async void SignUpCommandExecute(SignUpView view)
        {
            if (view.PasswordBox.Password != view.PasswordBoxRepeat.Password)
            {
                GlobalMessageQueue.Enqueue("Password does not match the confirm password.");
                return;
            }

            await SignUpAsync(new SignUpInfoBody
            {
                Email = EmailAddress,
                Username = Username,
                Password = view.PasswordBox.Password.ToMd5().EncryptByRsa()
            }, () =>
            {
                EmailAddress = string.Empty;
                Username = string.Empty;
                view.PasswordBox.Password = string.Empty;
                view.PasswordBoxRepeat.Password = string.Empty;
            });
        }

        private async Task SignUpAsync(SignUpInfoBody signUpInfo, Action successAction)
        {
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);
            var nonAuthApi = Container.Resolve<INonAuthenticationApi>();
            await nonAuthApi.SignUpAsync(signUpInfo).RunApi(() =>
            {
                successAction?.Invoke();
                GlobalMessageQueue.Enqueue("Registered successfully!");
                EventAggregator.GetEvent<SignUpSuccessEvent>().Publish(signUpInfo);
            });
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
        }
    }
}
