using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class ModuleBase : IModule
    {
        private readonly IUnityContainer _container;

        protected ModuleBase(IUnityContainer container)
        {
            _container = container;
        }

        public abstract void Initialize();

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
