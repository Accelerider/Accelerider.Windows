using System;
using System.Threading.Tasks;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.ViewModels.Items
{
    public class TransferTaskViewModel : BindableBase
    {
        private string _ownerName;
        private DataSize _progress;
        private TimeSpan? _remainingTime;
        private DataSize _speed;

        private bool _isBusy;


        public TransferTaskViewModel(ITransferTaskToken token)
        {
            OwnerName = token.OwnerName;
            Token = token;
            Token.TransferTaskStatusChanged += (sender, e) => OnPropertyChanged(nameof(TransferTaskStatus));

            RefreshTransferTaskStatusCycle();
        }


        public string OwnerName
        {
            get => _ownerName;
            set => SetProperty(ref _ownerName, value);
        }

        public ITransferTaskToken Token { get; }

        public IFileSummary FileSummary => Token.FileSummary;

        public TransferTaskStatusEnum TransferTaskStatus => Token.TaskStatus;

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


        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }


        private async void RefreshTransferTaskStatusCycle()
        {
            while (true)
            {
                await Task.Delay(1000);
                UpdateTransferTaskStatus();
            }
        }

        private void UpdateTransferTaskStatus()
        {
            Speed = Token.Progress - Progress;
            Progress = Token.Progress;
            if (Speed.BaseBValue == 0)
                RemainingTime = null;
            else
                RemainingTime = TimeSpan.FromSeconds(Math.Round((Token.FileSummary.FileSize - Progress) / Speed));
        }
    }
}
