using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.AppStore
{
    public class AppStoreComponentViewModel : ViewModelBase
    {
        public AppStoreComponentViewModel(IUnityContainer container) : base(container) { }
    }
}
