using Accelerider.Windows.Infrastructure;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Components.Authenticator
{
    public class Module : ModuleBase
    {
        public Module(IUnityContainer container) : base(container)
        {
        }

        public override void Initialize()
        {
            RegisterTypeIfMissing<IAuthenticator<IOneDriveUser>, AuthenticatorOneDrive>(true);
            RegisterTypeIfMissing<IAuthenticator<IBaiduCloudUser>, AuthenticatorBaiduCloud>(true);
        }
    }
}
