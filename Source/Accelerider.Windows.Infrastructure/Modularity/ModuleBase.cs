using Prism.Ioc;
using Prism.Modularity;
using Unity;


namespace Accelerider.Windows.Infrastructure.Modularity
{
    public abstract class ModuleBase : IModule
    {
        protected IUnityContainer Container { get; }

        protected ModuleBase(IUnityContainer container) => Container = container;

        public virtual void RegisterTypes(IContainerRegistry containerRegistry) { }

        public virtual void OnInitialized(IContainerProvider containerProvider) { }
    }
}
