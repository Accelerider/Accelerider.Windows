using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
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
        private string _username;
        private ICommand _signUpCommand;


        public SignUpViewModel(IUnityContainer container) : base(container)
        {
            SignUpCommand = new RelayCommand<SignUpView>(SignUpCommandExecute, SignUpCommandCanExecute);
        }

        private bool SignUpCommandCanExecute(SignUpView view) => !string.IsNullOrEmpty(Username) &&
                                                                 !string.IsNullOrEmpty(view.PasswordBox.Password) &&
                                                                 !string.IsNullOrEmpty(view.PasswordBoxRepeat.Password) &&
                                                                 !string.IsNullOrEmpty(view.PasswordBoxCode.Password);

        private async void SignUpCommandExecute(SignUpView view)
        {
            if (view.PasswordBox.Password != view.PasswordBoxRepeat.Password)
            {
                GlobalMessageQueue.Enqueue("Password does not match the confirm password.");
                return;
            }
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);



            try
            {
                var nonAuthApi = Container.Resolve<INonAuthenticationApi>();

                var publickeyPath = Path.Combine(Environment.CurrentDirectory , "publickey.xml");
                if (!File.Exists(publickeyPath))
                {
                    var publickey = await nonAuthApi.GetPublicKeyAsync();
                    File.WriteAllText(publickeyPath, publickey);
                }

                var passwordEn1 = "My password".ToMd5();
                var passwordEn2 = passwordEn1.EncryptByRijndael();
                var passwordDe1 = passwordEn2.DecryptByRijndael();
                var passwordEn3 = passwordDe1.EncryptByRsa();
                var passwordDe2 = passwordEn3.DecryptByRsa();

                var temp = await nonAuthApi.SignUpAsync(new SignUpInfoBody
                {
                    Email = "787673395@qq.com",
                    Password = passwordEn3,
                    Username = "No B Tree"
                });
                var temp1 = await nonAuthApi.LoginAsync(new LoginInfoBody {Username = "No B Tree", Password = passwordEn3 });
            }
            catch (ApiException e)
            {
                Debug.WriteLine(e.Content);
            }
            catch (HttpRequestException httpRequestException)
            {
                Debug.WriteLine(httpRequestException.Message);
            }
            //var message = await AcceleriderUser.SignUpAsync(Username, view.PasswordBox.Password, view.PasswordBoxCode.Pas sword);
            //if (string.IsNullOrEmpty(message))
            //{
            //    GlobalMessageQueue.Enqueue("You have successfully registered!");

            //}
            //else
            //{
            //    GlobalMessageQueue.Enqueue(message);
            //}
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
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
