using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;
using FirstFloor.ModernUI.Presentation;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Assets;
using System.Windows.Media;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    public class AppearanceOptionAreaViewModel : ViewModelBase
    {
        private readonly ILocalConfigInfo _localConfigInfo;

        private Link _selectedTheme;
        private Color _selectedPalette;

        public AppearanceOptionAreaViewModel(IUnityContainer container, ILocalConfigInfo localConfigInfo) : base(container)
        {
            _localConfigInfo = localConfigInfo;

            var temp = AppearanceManager.Current.AccentColor;
            SelectedTheme = Themes.FirstOrDefault(item => item.DisplayName == _localConfigInfo.Theme);
            SelectedPalette = temp;
        }

        public LinkCollection Themes => StaticResources.Themes;
        public ObservableCollection<Color> Palettes => StaticResources.AccentColors;

        public Link SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                if (SetProperty(ref _selectedTheme, value))
                {
                    AppearanceManager.Current.ThemeSource = value.Source;
                    SelectedPalette = AppearanceManager.Current.AccentColor;
                    _localConfigInfo.Theme = value.DisplayName;
                }
            }
        }
        public Color SelectedPalette
        {
            get { return AppearanceManager.Current.AccentColor; }
            set
            {
                if (SetProperty(ref _selectedPalette, value))
                {
                    AppearanceManager.Current.AccentColor = value;
                    _localConfigInfo.Background = $"{value.R},{value.G},{value.B}";
                }
            }
        }
    }
}
