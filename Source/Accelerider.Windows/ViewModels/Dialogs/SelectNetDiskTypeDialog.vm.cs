using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Accelerider.Windows.Commands;
using Accelerider.Windows.ViewModels.Items;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.ViewModels.Dialogs
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
                    Logo = new BitmapImage(new Uri(@"..\..\Assets\Images\BaiduCloudLogo.png", UriKind.Relative)),
                    Name = "Baidu Cloud",
                    Description = "It is a stupid net-disk."
                },
                new NetDiskTypeViewModel
                {
                    Logo = new BitmapImage(new Uri(@"..\..\Assets\Images\OneDriveLogo.jpg", UriKind.Relative)),
                    Name = "OneDrive",
                    Description = "It is a stupid net-disk."
                },
            };
        }
    }
}
