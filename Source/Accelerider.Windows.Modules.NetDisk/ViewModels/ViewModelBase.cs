using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels
{
    public abstract class ViewModelBase : Infrastructure.ViewModelBase
    {
        protected ViewModelBase(IUnityContainer container) : base(container) { }

        public INetDiskUser NetDiskUser
        {
            get => AcceleriderUser.CurrentNetDiskUser ?? AcceleriderUser.NetDiskUsers.FirstOrDefault();
            set
            {
                var temp = AcceleriderUser.CurrentNetDiskUser;
                if (!SetProperty(ref temp, value)) return;
                AcceleriderUser.CurrentNetDiskUser = temp;
                EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Publish(temp);
            }
        }
    }
}
