using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal abstract class ViewModelBase : BindableBase
    {
        protected IUnityContainer Container { get; }
        protected IEventAggregator EventAggregator { get; }

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
            EventAggregator = container.Resolve<IEventAggregator>();
        }
    }
}
