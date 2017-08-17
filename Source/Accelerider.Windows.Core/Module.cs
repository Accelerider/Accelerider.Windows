using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Core
{
    public class Module
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            RegisterTypeIfMissing<ILocalConfigureInfo, LocalConfigureInfo>(true);
            RegisterTypeIfMissing<IAcceleriderUser, AcceleriderUser>(true);
            RegisterTypeIfMissing<INetDiskUser, NetDiskUser>(false);
        }

        private void RegisterTypeIfMissing<TForm, TTo>(bool registerAsSingleton) where TTo : TForm
        {
            if (_container.IsRegistered<TForm>()) return;

            if (registerAsSingleton)
            {
                _container.RegisterType<TForm, TTo>(new ContainerControlledLifetimeManager());
            }
            else
            {
                _container.RegisterType<TForm, TTo>();
            }
        }
    }
}
