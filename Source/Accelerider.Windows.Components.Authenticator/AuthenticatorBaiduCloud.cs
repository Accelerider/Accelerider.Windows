using Accelerider.Windows.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Accelerider.Windows.Components.Authenticator
{
    public class AuthenticatorBaiduCloud : IAuthenticator<IBaiduCloudUser>
    {
        public Task<IBaiduCloudUser> Authenticate()
        {
            throw new NotImplementedException();
        }

        public UserControl GetAuthenticatorView()
        {
            var browser = new AuthenticationBrowser();
            // TODO: set Source property.
            return browser;
        }
    }
}
