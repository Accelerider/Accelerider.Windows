using System;
using System.Threading.Tasks;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.ViewModels.Items
{
    public class TransferTaskViewModel : BindableBase
    {
        private readonly ITransferTaskToken _token;

        private DataSize _progress;
        private TimeSpan? _remainingTime;
        private DataSize _speed;

        private RelayCommandAsync _cancelCommand;
        private RelayCommandAsync _restartCommand;
        private RelayCommandAsync _pauseCommand;
        private bool _isTransferStateChanging;

        public TransferTaskViewModel(ITransferTaskToken token)
        {
            _token = token;
            _token.TransferStateChanged += (sender, e) => OnPropertyChanged(nameof(TransferState));

            CancelCommand = new RelayCommandAsync(
                _token.CancelAsync,
                () => _token.TransferState.CanChangeTo(TransferStateEnum.Canceled));
            RestartCommand = new RelayCommandAsync(
                _token.RestartAsync,
                () => _token.TransferState.CanChangeTo(TransferStateEnum.Waiting));
            PauseCommand = new RelayCommandAsync(
                _token.PauseAsync,
                () => _token.TransferState.CanChangeTo(TransferStateEnum.Paused));

            RefreshTransferState();
        }

        public IDiskFile FileInfo => _token.FileInfo;

        public TransferStateEnum TransferState => _token.TransferState;

        public DataSize Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public DataSize Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }

        public TimeSpan? RemainingTime
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }


        public RelayCommandAsync CancelCommand
        {
            get => _cancelCommand;
            set => SetProperty(ref _cancelCommand, value);
        }

        public RelayCommandAsync RestartCommand
        {
            get => _restartCommand;
            set => SetProperty(ref _restartCommand, value);
        }

        public RelayCommandAsync PauseCommand
        {
            get => _pauseCommand;
            set => SetProperty(ref _pauseCommand, value);
        }

        public bool IsTransferStateChanging
        {
            get => _isTransferStateChanging;
            set => SetProperty(ref _isTransferStateChanging, value);
        }

        private async void RefreshTransferState()
        {
            while (_token.TransferState == TransferStateEnum.Transfering)
            {
                await Task.Delay(1000);
                UpdateTransferState();
            }
        }

        private void UpdateTransferState()
        {
            Speed = _token.Progress - Progress;
            Progress = _token.Progress;
            if (Speed.BaseBValue == 0)
                RemainingTime = null;
            else
                RemainingTime = TimeSpan.FromSeconds(Math.Round((_token.FileInfo.FileSize - Progress) / Speed));
        }
    }
}
