using System.ComponentModel;
using System.Linq;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models;
using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels
{
    public abstract class ViewModelBase : Infrastructure.Mvvm.ViewModelBase
    {
        protected ViewModelBase(IUnityContainer container) : base(container)
        {
            AcceleriderUserExtensions.Register(this);
        }

        public INetDiskUser CurrentNetDiskUser
        {
            get
            {
                if (AcceleriderUser.GetCurrentNetDiskUser() == null)
                {
                    CurrentNetDiskUser = AcceleriderUser.GetNetDiskUsers().FirstOrDefault();
                }

                return AcceleriderUser.GetCurrentNetDiskUser();
            }
            set { if (AcceleriderUser.SetCurrentNetDiskUser(value)) OnCurrentNetDiskUserChanged(value); }
        }

        public virtual void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentNetDiskUser))
                OnCurrentNetDiskUserChanged(CurrentNetDiskUser);

            RaisePropertyChanged(e.PropertyName);
        }

        protected virtual void OnCurrentNetDiskUserChanged(INetDiskUser value) { }
    }
}
