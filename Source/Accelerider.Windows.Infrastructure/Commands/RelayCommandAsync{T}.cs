using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure
{
    public class RelayCommandAsync<T> : RelayCommandBase, INotifyPropertyChanged
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private bool _isExecuting;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting == value) return;
                _isExecuting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExecuting)));
            }
        }


        public RelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }


        protected override bool CanExecute(object parameter) => CanExecute((T)parameter) && !IsExecuting;

        protected override async void Execute(object parameter) => await Execute((T)parameter);

        public bool CanExecute(T parameter) => _canExecute?.Invoke(parameter) ?? true;

        public async Task Execute(T parameter)
        {
            IsExecuting = true;
            var invoke = _execute?.Invoke(parameter);
            if (invoke != null) await invoke;
            IsExecuting = false;
        }
    }
}
