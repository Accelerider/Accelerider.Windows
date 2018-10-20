using System;
using System.Windows.Input;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class RelayCommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        protected abstract void Execute(object parameter);
        protected abstract bool CanExecute(object parameter);
    }
}
