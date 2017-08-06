using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Accelerider.Windows.Commands
{
    public class RelayCommandAsync<T> : RelayCommandBase, INotifyPropertyChanged
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private bool _isWorking;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsWorking
        {
            get => _isWorking;
            private set
            {
                if (_isWorking == value) return;
                _isWorking = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWorking)));
            }
        }

        public RelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute((T)parameter) && !IsWorking;
        }

        protected override async void Execute(object parameter)
        {
            await Execute((T)parameter);
        }

        public bool CanExecute(T parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public async Task Execute(T parameter)
        {
            IsWorking = true;
            var invoke = _execute?.Invoke(parameter);
            if (invoke != null) await invoke;
            IsWorking = false;
        }
    }
}
