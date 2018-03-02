using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Models;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.AppStore
{
    public class AppStoreComponentViewModel : ViewModelBase
    {
        private IEnumerable<AcceleriderModule> _modules;

        public AppStoreComponentViewModel(IUnityContainer container) : base(container)
        {
            Modules = Container.Resolve<IConfigureFile>().GetValue<IEnumerable<AcceleriderModule>>("AcceleriderModules");
        }

        public IEnumerable<AcceleriderModule> Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }


    }
}
