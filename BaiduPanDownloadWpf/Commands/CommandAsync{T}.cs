using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Commands
{
    public class CommandAsync<T> : CommandBase, INotifyPropertyChanged
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private bool _isWorking;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                if (_isWorking == value) return;
                _isWorking = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWorking)));
            }
        }

        public CommandAsync(Func<T, Task> execute, Func<T, bool> canExecute = null)
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
            await _execute?.Invoke(parameter);
            IsWorking = false;
        }
    }
}
