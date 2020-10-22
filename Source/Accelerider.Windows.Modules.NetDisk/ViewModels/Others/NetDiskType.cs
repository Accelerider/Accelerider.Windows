using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Accelerider.Windows.Infrastructure;
using MaterialDesignThemes.Wpf;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Others
{
    public class NetDiskType : BindableBase
    {
        private BitmapImage _logo;
        private string _name;
        private string _description;

        public NetDiskType()
        {
            OpenCommand = new RelayCommandAsync(async () =>
            {
                DialogHost.CloseDialogCommand.Invoke();
                await TimeSpan.FromMilliseconds(500);
                if (OnClick != null) await OnClick();
            });
        }

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

        public Func<Task> OnClick { get; set; }

        public ICommand OpenCommand { get; }
    }
}
