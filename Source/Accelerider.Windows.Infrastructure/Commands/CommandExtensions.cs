using System.Windows.Input;

namespace Accelerider.Windows.Infrastructure.Commands
{
    public static class CommandExtensions
    {
        public static void Invoke(this ICommand @this, object parameter = null)
        {
            if (@this != null && @this.CanExecute(parameter)) @this.Execute(parameter);
        }
    }
}
