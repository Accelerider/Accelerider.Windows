using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Others
{
    public class TransportingTaskItem : BindableBase
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };

        private string _ownerName;
        private DataSize _progress;
        private TimeSpan? _remainingTime;
        private DataSize _speed;
        private bool _isBusy;

        public TransportingTaskItem(ITransportTask token)
        {
            OwnerName = token.OwnerName;
            Token = token;
            Token.StatusChanged += (sender, e) => RaisePropertyChanged(nameof(TransferTaskStatus));

            _timer.Tick += RefreshTransferTaskStatus;
            _timer.Start();
        }

        public string OwnerName
        {
            get => _ownerName;
            set => SetProperty(ref _ownerName, value);
        }

        public ITransportTask Token { get; }

        public IFileSummary FileSummary => Token.FileSummary;

        public TransportStatus TransferTaskStatus => Token.Status;

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

        private void RefreshTransferTaskStatus(object sender, EventArgs e)
        {
            Speed = Token.CompletedSize - Progress;
            Progress = Token.CompletedSize;
            if (Speed.BaseBValue == 0)
                RemainingTime = null;
            else
                RemainingTime = TimeSpan.FromSeconds(Math.Round((Token.Size - Progress) / Speed));
        }
    }
}
