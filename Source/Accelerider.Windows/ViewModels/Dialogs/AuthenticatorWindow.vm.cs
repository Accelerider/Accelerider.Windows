using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Infrastructure.Interfaces;
using System.Windows;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class AuthenticatorWindowViewModel : ViewModelBase
    {
        private object _content;
        private Func<Task<bool>> _authenticate;

        public AuthenticatorWindowViewModel(IUnityContainer container) : base(container)
        {
        }

        public object Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }


        public override async void OnLoaded(object view)
        {
            var window = view as Window;
            try
            {
                window.DialogResult = await _authenticate?.Invoke();
            }
            catch (InvalidOperationException) { }
        }


        public void SetAuthenticator<T>(IAuthenticator<T> authenticator) where T : INetDiskUser
        {
            Content = authenticator.GetAuthenticatorView();
            _authenticate = async () =>
            {
                var user = await authenticator.Authenticate();
                return await Container.Resolve<IAcceleriderUser>().AddNetDiskUserAsync(user);
            };
        }
    }
}
