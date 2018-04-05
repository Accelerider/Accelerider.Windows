using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class ModuleBase : IModule
    {
        protected IUnityContainer Container { get; }

        protected ModuleBase(IUnityContainer container) => Container = container;

        public abstract void Initialize();

        protected void RegisterTypeIfMissing<TForm, TTo>(bool registerAsSingleton) where TTo : TForm
        {
            if (Container.IsRegistered<TForm>()) return;

            if (registerAsSingleton)
            {
                Container.RegisterType<TForm, TTo>(new ContainerControlledLifetimeManager());
            }
            else
            {
                Container.RegisterType<TForm, TTo>();
            }
        }
    }
}
