using Accelerider.Windows.Core;
using Accelerider.Windows.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Accelerider.Windows.Components.Authenticator
{
    public class AuthenticatorOneDrive : IAuthenticator<IOneDriveUser>
    {
        public async Task<IOneDriveUser> Authenticate()
        {
            await Task.Delay(3000);
            return new OneDriveUser();
        }

        public UserControl GetAuthenticatorView()
        {
            var browser = new AuthenticationBrowser();
            // TODO: set Source property.
            return browser;
        }
    }
}
