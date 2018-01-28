using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Commands
{
    public class CommandAsync : CommandBase, INotifyPropertyChanged
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
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


        public CommandAsync(Func<Task> execute, Func<bool> canExecute = null)
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
            await _execute?.Invoke();
            IsWorking = false;
        }
    }
}
