namespace Accelerider.Windows.Infrastructure.ViewModels
{
    public interface ILocalizable
    {
        I18nManager I18nManager { get; set; }

        void OnCurrentUICultureChanged();
    }
}
