using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Models;
using Unity;


namespace Accelerider.Windows.ViewModels.AppStore
{
    public class AppStoreComponentViewModel : ViewModelBase
    {
        public AppStoreComponentViewModel(IUnityContainer container) : base(container)
        {
        }
    }
}
