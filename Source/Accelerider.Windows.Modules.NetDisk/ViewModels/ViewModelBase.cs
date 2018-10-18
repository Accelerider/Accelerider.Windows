using System.Linq;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Autofac;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels
{
    public abstract class ViewModelBase : Infrastructure.Mvvm.ViewModelBase
    {
        protected ViewModelBase(IContainer container) : base(container) { }

        public INetDiskUser CurrentNetDiskUser
        {
            get => AcceleriderUser.GetCurrentNetDiskUser() ?? AcceleriderUser.GetNetDiskUsers().FirstOrDefault();
            set
            {
                var temp = AcceleriderUser.GetCurrentNetDiskUser();
                if (!SetProperty(ref temp, value)) return;
                AcceleriderUser.SetCurrentNetDiskUser(value);
                EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Publish(value);
            }
        }
    }
}
