using System.Globalization;

namespace Accelerider.Windows.Infrastructure.ViewModels
{
    public interface ILocalizable
    {
        void OnCurrentUICultureChanged(CultureInfo currentCultureInfo);
    }
}
