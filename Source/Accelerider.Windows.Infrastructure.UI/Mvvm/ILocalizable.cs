using WpfExtensions.Xaml;

namespace Accelerider.Windows.Infrastructure.Mvvm
{
    public interface ILocalizable
    {
        I18nManager I18nManager { get; set; }

        void OnCurrentUICultureChanged(object sender, CurrentUICultureChangedEventArgs e);
    }
}
