using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using System.IO;

namespace BaiduPanDownloadWpf.Core
{
    public class DownloadCoreModule : IModule
    {
        private readonly IUnityContainer _container;

        public DownloadCoreModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            if (!Directory.Exists(Common.UserDataSavePath)) Directory.CreateDirectory(Common.UserDataSavePath);
            _container.RegisterInstance(typeof(ILocalConfigInfo), LocalConfigInfo.Create());
            RegisterTypeIfMissing<IMountUserRepository, MountUserRepository>(true);
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <typeparam name="TForm">The interface type to register.</typeparam>
        /// <typeparam name="TTo">The type implementing the interface.</typeparam>
        /// <param name="registerAsSingleton">Registers the type as a singleton.</param>
        protected void RegisterTypeIfMissing<TForm, TTo>(bool registerAsSingleton) where TTo : TForm
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
