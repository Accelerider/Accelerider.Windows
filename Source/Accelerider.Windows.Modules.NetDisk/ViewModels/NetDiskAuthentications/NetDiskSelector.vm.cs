using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Accelerider.Windows.Modules.NetDisk.Views.NetDiskAuthentications;
using Prism.Regions;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.NetDiskAuthentications
{
    public class NetDiskSelectorViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private bool _isHome;

        public ICommand BackCommand { get; }

        public bool IsHome
        {
            get => _isHome;
            set => SetProperty(ref _isHome, value);
        }


        public NetDiskSelectorViewModel(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            IsHome = true;
            _regionManager = regionManager;
            NetDiskTypes = InitializeNetDiskTypes();

            SelectNetDiskCommand = new RelayCommand<NetDiskType>(SelectNetDiskCommandExecute);
            BackCommand = new RelayCommand(() =>
            {
                _regionManager.RequestNavigate(Constants.NetDiskAuthenticationViewRegion, nameof(NetDiskList));
                IsHome = true;
            });
        }

        private IEnumerable<NetDiskType> _netDiskTypes;
        private ICommand _selectNetDiskCommand;

        public IEnumerable<NetDiskType> NetDiskTypes
        {
            get => _netDiskTypes;
            set => SetProperty(ref _netDiskTypes, value);
        }

        public ICommand SelectNetDiskCommand
        {
            get => _selectNetDiskCommand;
            set => SetProperty(ref _selectNetDiskCommand, value);
        }


        private void SelectNetDiskCommandExecute(NetDiskType netDiskType)
        {
        }

        // HACK: Mock data.
        private IEnumerable<NetDiskType> InitializeNetDiskTypes()
        {
            return new[]
            {
                new NetDiskType
                {
                    Logo = new BitmapImage(new Uri(@"..\..\Images\logo-baiducloud.png", UriKind.Relative)),
                    Name = "Baidu Cloud",
                    Description = "XXXXXXXXXXXXXXXXXXXXXXXXXXX"
                },
                new NetDiskType
                {
                    Logo = new BitmapImage(new Uri(@"..\..\Images\logo-onedrive.jpg", UriKind.Relative)),
                    Name = "OneDrive",
                    Description = "XXXXXXXXXXXXXXXXXXXXXXXXXXX"
                },
                new NetDiskType
                {
                    Logo = new BitmapImage(new Uri(@"..\..\Images\logo-sixcloud.png", UriKind.Relative)),
                    Name = "Six Cloud",
                    Description = "XXXXXXXXXXXXXXXXXXXXXXXXXXX",
                    OnClick = () =>
                    {
                        _regionManager.RequestNavigate(Constants.NetDiskAuthenticationViewRegion, nameof(SixCloud));
                        IsHome = false; 
                        return Task.CompletedTask;
                    }
                },
            };
        }
    }
}
