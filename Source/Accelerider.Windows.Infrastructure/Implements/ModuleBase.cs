using System;
using Autofac;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class ModuleBase : IModule
    {
        protected IContainer Container { get; }

        protected ModuleBase(IContainer container) => Container = container;

        public virtual void Initialize()
        {
            var builder = new ContainerBuilder();
            ConfigureContainerBuilder(builder);
            builder.Update(Container);
        }

        protected virtual void ConfigureContainerBuilder(ContainerBuilder builder)
        {
        }

        protected void RegisterTypeIfMissing<TForm, TTarget>(ContainerBuilder builder, bool registerAsSingleton) where TTarget : TForm
        {
            if (Container.IsRegistered<TForm>()) return;

            if (registerAsSingleton)
            {
                builder.RegisterType<TTarget>().As<TForm>().SingleInstance();
            }
            else
            {
                builder.RegisterType<TTarget>().As<TForm>();
            }
        }
    }
}
