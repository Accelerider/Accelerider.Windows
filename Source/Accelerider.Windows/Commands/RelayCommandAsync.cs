using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Accelerider.Windows.Commands
{
    public class RelayCommandAsync : RelayCommandBase, INotifyPropertyChanged
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
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


        public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute() && !IsWorking;
        }

        protected override async void Execute(object parameter)
        {
            await Execute();
        }

        public bool CanExecute()
        {
            return _canExecute?.Invoke() ?? true;
        }

        public async Task Execute()
        {
            IsWorking = true;
            var invoke = _execute?.Invoke();
            if (invoke != null) await invoke;
            IsWorking = false;
        }
    }
}
