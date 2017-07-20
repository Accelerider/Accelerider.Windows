using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        protected IUnityContainer Container { get; }

        protected ViewModelBase(IUnityContainer container)
        {
            Container = container;
        }


    }
}
