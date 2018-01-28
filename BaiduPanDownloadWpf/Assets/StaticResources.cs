using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BaiduPanDownloadWpf.Assets
{
    internal static class StaticResources
    {
        public const string DarkThemeKey = "夜间主题";
        public const string LightThemeKey = "纯白主题";
        public const string YourNameThemeKey = "你的名字";
        public const string NewYearThemeKey = "鸡年大吉";


        public static readonly LinkCollection Themes = InitializeThemes();
        public static readonly ObservableCollection<Color> AccentColors = InitializeAccentColors();


        private static LinkCollection InitializeThemes()
        {
            var temp = new LinkCollection();
            temp.Add(new Link { DisplayName = DarkThemeKey, Source = AppearanceManager.DarkThemeSource });
            temp.Add(new Link { DisplayName = LightThemeKey, Source = AppearanceManager.LightThemeSource });
            temp.Add(new Link { DisplayName = YourNameThemeKey, Source = new Uri("/Assets/Themes/ModernUI.YourNameTheme.xaml", UriKind.Relative) });
            temp.Add(new Link { DisplayName = NewYearThemeKey, Source = new Uri("/Assets/Themes/ModernUI.NewYearTheme.xaml", UriKind.Relative) });
            return temp;
        }
        private static ObservableCollection<Color> InitializeAccentColors()
        {
            return new ObservableCollection<Color>
            {
                Color.FromRgb(0xa4, 0xc4, 0x00), // lime
                Color.FromRgb(0x60, 0xa9, 0x17), // green
                Color.FromRgb(0x00, 0x8a, 0x00), // emerald
                Color.FromRgb(0x00, 0xab, 0xa9), // teal
                Color.FromRgb(0x1b, 0xa1, 0xe2), // cyan
                Color.FromRgb(0x00, 0x50, 0xef), // cobalt
                Color.FromRgb(0x6a, 0x00, 0xff), // indigo
                Color.FromRgb(0xaa, 0x00, 0xff), // violet
                Color.FromRgb(0xf4, 0x72, 0xd0), // pink
                Color.FromRgb(0xd8, 0x00, 0x73), // magenta
                Color.FromRgb(0xa2, 0x00, 0x25), // crimson
                Color.FromRgb(0xe5, 0x14, 0x00), // red
                Color.FromRgb(0xfa, 0x68, 0x00), // orange
                Color.FromRgb(0xf0, 0xa3, 0x0a), // amber
                Color.FromRgb(0xe3, 0xc8, 0x00), // yellow
                Color.FromRgb(0x82, 0x5a, 0x2c), // brown
                Color.FromRgb(0x6d, 0x87, 0x64), // olive
                Color.FromRgb(0x64, 0x76, 0x87), // steel
                Color.FromRgb(0x76, 0x60, 0x8a), // mauve
                Color.FromRgb(0x87, 0x79, 0x4e), // taupe
                Color.FromRgb(0xc9, 0x46, 0x26), // new year theme
            };
        }
    }
}
