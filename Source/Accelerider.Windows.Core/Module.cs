using System.Net;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Core
{
    public class Module : ModuleBase
    {
        public Module(IUnityContainer container) : base(container)
        {
        }

        public override void Initialize()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
            RegisterTypeIfMissing<ILocalConfigureInfo, LocalConfigureInfo>(true);
            RegisterTypeIfMissing<IAcceleriderUser, AcceleriderUser>(true);
        }
    }
}
