using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.Infrastructure.Mvvm
{
    public interface INotificable
    {
        ISnackbarMessageQueue GlobalMessageQueue { get; set; }
    }
}
