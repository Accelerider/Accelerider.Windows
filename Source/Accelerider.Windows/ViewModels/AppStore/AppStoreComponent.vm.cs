using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Models;
using Autofac;


namespace Accelerider.Windows.ViewModels.AppStore
{
    public class AppStoreComponentViewModel : ViewModelBase
    {
        private IEnumerable<ModuleMetadata> _modules;

        public AppStoreComponentViewModel(IContainer container) : base(container)
        {
            Modules = Container.Resolve<IConfigureFile>().GetValue<IEnumerable<ModuleMetadata>>("AcceleriderModules");
        }

        public IEnumerable<ModuleMetadata> Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }


    }
}
