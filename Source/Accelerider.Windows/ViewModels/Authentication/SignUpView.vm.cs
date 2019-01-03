using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.ServerInteraction;
using MaterialDesignThemes.Wpf;
using Refit;
using Unity;

namespace Accelerider.Windows.ViewModels.Authentication
{
    public class SignUpViewModel : ViewModelBase, INotificable
    {
        private const int IntervalBasedSecond = 60;

        private string _emailAddress;
        private SignUpArgs _signUpArgs;
        private int _remainingTimeBasedSecond;
        private bool _isLocked;

        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        public int RemainingTimeBasedSecond
        {
            get => _remainingTimeBasedSecond;
            set => SetProperty(ref _remainingTimeBasedSecond, value);
        }

        public string EmailAddress
        {
            get => _emailAddress;
            set => SetProperty(ref _emailAddress, value);
        }

        public string Username
        {
            get => _signUpArgs.Username;
            set
            {
                var temp = _signUpArgs.Username;
                if (SetProperty(ref temp, value))
                {
                    _signUpArgs.Username = temp;
                }
            }
        }

        public string VerificationCode
        {
            get => _signUpArgs.VerificationCode;
            set
            {
                var temp = _signUpArgs.VerificationCode;
                if (SetProperty(ref temp, value))
                {
                    _signUpArgs.VerificationCode = temp;
                }
            }
        }

        public ICommand SignUpCommand { get; }

        public ICommand SendVerificationCodeCommand { get; }

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        public SignUpViewModel(IUnityContainer container, INonAuthenticationApi nonAuthenticationApi) : base(container)
        {
            ConfigureSignUpArgs();

            SignUpCommand = new RelayCommand<PasswordBox>(SignUpCommandExecute, SignUpCommandCanExecute);
            SendVerificationCodeCommand = new RelayCommandAsync(
                async () =>
                {
                    try
                    {
                        var result = await nonAuthenticationApi.SendVerificationCodeAsync(EmailAddress);
                        if (result.Success)
                        {
                            ConfigureSignUpArgs(args => args.SessionId = result.SessionId);
                            await StartVerificationCodeTimer();
                        }
                    }
                    catch (ApiException e)
                    {
                        GlobalMessageQueue.Enqueue(e.Content.ToObject<ResponseBase>()?.Status);
                    }
                },
                () => EmailAddress.IsEmailAddress() &&
                      !IsLocked);
        }

        private bool SignUpCommandCanExecute(PasswordBox passwordBox) => new[]
        {
            EmailAddress,
            Username,
            passwordBox.Password,
        }.All(field => !string.IsNullOrEmpty(field));

        private async void SignUpCommandExecute(PasswordBox passwordBox)
        {
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);
            var nonAuthApi = Container.Resolve<INonAuthenticationApi>();

            ConfigureSignUpArgs(configure: args => args.Password = passwordBox.Password);
            await nonAuthApi.SignUpAsync(_signUpArgs).RunApi(() =>
            {
                ConfigureSignUpArgs(force: true);
                passwordBox.Password = string.Empty;

                GlobalMessageQueue.Enqueue("Registered successfully!");
                EventAggregator.GetEvent<SignUpSuccessEvent>().Publish(_signUpArgs);
            });
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
        }

        private void ConfigureSignUpArgs(Action<SignUpArgs> configure = null, bool force = false)
        {
            if (_signUpArgs == null || force) _signUpArgs = new SignUpArgs();

            configure?.Invoke(_signUpArgs);
        }

        private async Task StartVerificationCodeTimer()
        {
            IsLocked = true;
            for (RemainingTimeBasedSecond = IntervalBasedSecond; RemainingTimeBasedSecond > 0; RemainingTimeBasedSecond--)
            {
                await TimeSpan.FromSeconds(1);
            }
            IsLocked = false;
        }
    }
}
