using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine;
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
            RegisterTypeIfMissing<ILocalConfigureInfo, LocalConfigureInfo>(true);
            RegisterTypeIfMissing<IAcceleriderUser, AcceleriderUser>(true);
        }
    }
}
