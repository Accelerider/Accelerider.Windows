using Microsoft.Practices.Unity;
using Prism.Events;

namespace BaiduPanDownloadWpf.Core
{
    public abstract class ModelBase
    {
        protected IUnityContainer Container { get; }
        protected IEventAggregator EventAggregator { get; }

        protected ModelBase(IUnityContainer container)
        {
            Container = container.CreateChildContainer();
            EventAggregator = container.Resolve<IEventAggregator>();
        }
    }
}
