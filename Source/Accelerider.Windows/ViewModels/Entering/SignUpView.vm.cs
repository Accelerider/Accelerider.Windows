using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Common;
using Accelerider.Windows.Views.Entering;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Entering
{
    public class SignUpViewModel : ViewModelBase
    {
        private string _username;
        private ICommand _signUpCommand;


        public SignUpViewModel(IUnityContainer container) : base(container)
        {
            SignUpCommand = new RelayCommand<SignUpView>(SignUpCommandExecute, SignUpCommandCanExecute);
        }

        private bool SignUpCommandCanExecute(SignUpView view)
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(view.PasswordBox.Password) &&
                   !string.IsNullOrEmpty(view.PasswordBoxRepeat.Password) &&
                   !string.IsNullOrEmpty(view.PasswordBoxCode.Password);
        }

        private async void SignUpCommandExecute(SignUpView view)
        {
            if (view.PasswordBox.Password != view.PasswordBoxRepeat.Password)
            {
                GlobalMessageQueue.Enqueue("Password does not match the confirm password.");
                return;
            }
            EventAggregator.GetEvent<IsLoadingMainWindowEvent>().Publish(true);
            var message = await AcceleriderUser.SignUpAsync(Username, view.PasswordBox.Password, view.PasswordBoxCode.Password);
            if (string.IsNullOrEmpty(message))
            {
                GlobalMessageQueue.Enqueue("You have successfully registered!");

            }
            else
            {
                GlobalMessageQueue.Enqueue(message);
            }
            EventAggregator.GetEvent<IsLoadingMainWindowEvent>().Publish(false);
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
    }
}
