using System.Windows.Media.Imaging;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Others
{
    public class NetDiskTypeViewModel : BindableBase
    {
        private BitmapImage _logo;
        private string _name;
        private string _description;

        public BitmapImage Logo
        {
            get => _logo;
            set => SetProperty(ref _logo, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
