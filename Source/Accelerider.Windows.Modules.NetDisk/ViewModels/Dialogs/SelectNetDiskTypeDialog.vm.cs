using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Accelerider.Windows.Modules.NetDisk.Views.Dialogs;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Dialogs
{
    public class SelectNetDiskTypeDialogViewModel : ViewModelBase
    {
        private IEnumerable<NetDiskTypeViewModel> _netDiskTypes;
        private ICommand _selectNetDiskCommand;


        public SelectNetDiskTypeDialogViewModel(IUnityContainer container) : base(container)
        {
            NetDiskTypes = InitializeNetDiskTypes();
            SelectNetDiskCommand = new RelayCommand<NetDiskTypeViewModel>(SelectNetDiskCommandExecute);
        }

        public IEnumerable<NetDiskTypeViewModel> NetDiskTypes
        {
            get => _netDiskTypes;
            set => SetProperty(ref _netDiskTypes, value);
        }

        public ICommand SelectNetDiskCommand
        {
            get => _selectNetDiskCommand;
            set => SetProperty(ref _selectNetDiskCommand, value);
        }


        private void SelectNetDiskCommandExecute(NetDiskTypeViewModel netDiskType)
        {
            var window = new AuthenticatorWindow();
            switch (netDiskType.Name)
            {
                case "Baidu Cloud":
                    var authBaiduCloud = Container.Resolve<IAuthenticator<IBaiduCloudUser>>();
                    (window.DataContext as AuthenticatorWindowViewModel).SetAuthenticator(authBaiduCloud);
                    break;
                case "OneDrive":
                    var authOneDrive = Container.Resolve<IAuthenticator<IOneDriveUser>>();
                    (window.DataContext as AuthenticatorWindowViewModel).SetAuthenticator(authOneDrive);
                    break;
            }
            var dialogResult = window.ShowDialog();
        }

        private IEnumerable<NetDiskTypeViewModel> InitializeNetDiskTypes()
        {
            return new[]
            {
                new NetDiskTypeViewModel
                {
                    Logo = new BitmapImage(new Uri(@"..\..\Images\BaiduCloudLogo.png", UriKind.Relative)),
                    Name = "Baidu Cloud",
                    Description = "It is a stupid net-disk."
                },
                new NetDiskTypeViewModel
                {
                    Logo = new BitmapImage(new Uri(@"..\..\Images\OneDriveLogo.jpg", UriKind.Relative)),
                    Name = "OneDrive",
                    Description = "It is a stupid net-disk."
                },
            };
        }
    }
}
