using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Accelerider.Windows.ViewModels.Items;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class SeletNetDiskTypeDialogViewModel : ViewModelBase
    {
        private NetDiskTypeViewModel _netDiskType;
        private IEnumerable<NetDiskTypeViewModel> _netDiskTypes;


        public SeletNetDiskTypeDialogViewModel(IUnityContainer container) : base(container)
        {
            NetDiskTypes = InitializeNetDiskTypes();
        }


        public NetDiskTypeViewModel NetDiskType
        {
            get => _netDiskType;
            set { if (SetProperty(ref _netDiskType, value)) OnNetDiskTypeChanged(); }
        }

        public IEnumerable<NetDiskTypeViewModel> NetDiskTypes
        {
            get => _netDiskTypes;
            set => SetProperty(ref _netDiskTypes, value);
        }


        private void OnNetDiskTypeChanged()
        {
            var showDialog = new AuthenticationBrowserWindow().ShowDialog();
            if (showDialog != null && (bool)showDialog &&
                DialogHost.CloseDialogCommand.CanExecute(true, null))
            {
                DialogHost.CloseDialogCommand.Execute(true, null);
            }
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
