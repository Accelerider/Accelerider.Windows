namespace System.Windows.Input
{
    public static class CommandExtensions
    {
        public static void Invoke(this ICommand @this, object parameter = null)
        {
            if (@this != null && @this.CanExecute(parameter)) @this.Execute(parameter);
        }
    }
}
