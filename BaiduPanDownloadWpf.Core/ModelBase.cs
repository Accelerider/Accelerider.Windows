using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;

namespace BaiduPanDownloadWpf.Core
{
    public abstract class ModelBase
    {
        protected IUnityContainer Container { get; }
        protected IEventAggregator EventAggregator { get; }
        protected ILoggerFacade Logger { get; }


        protected ModelBase(IUnityContainer container)
        {
            Container = container.CreateChildContainer();
            EventAggregator = container.Resolve<IEventAggregator>();
            Logger = container.Resolve<ILoggerFacade>();
        }
    }
}
