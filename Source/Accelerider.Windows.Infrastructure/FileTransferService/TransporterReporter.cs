using System.Threading.Tasks;
using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public class TransporterReporter : BindableBase
    {
        private const int OneSecondDelay = 1000;

        private bool _isPaused;
        private DataSize _previousCompletedSize;

        private DataSize _speed;
        private double _progress;

        public ITransporter Transporter { get; private set; }

        public DataSize Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public DataSize CompletedSize => Transporter.CompletedSize;


        public TransporterReporter(ITransporter transporter)
        {
            Transporter = transporter;

            Transporter.StatusChanged += OnStatusChanged;

            if (Transporter.Status == TransferStatus.Transferring)
                PollTransporterInfoPerSecond(Transporter);
        }


        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {
            if (e.NewStatus == TransferStatus.Disposed)
            {
                Transporter.StatusChanged -= OnStatusChanged;
                Transporter = null;
                return;
            }

            _isPaused = e.NewStatus != TransferStatus.Transferring;

            if (_isPaused)
                UpdateTransporterInfo(0, Transporter.CompletedSize / Transporter.TotalSize);
            else
                PollTransporterInfoPerSecond(Transporter);
        }

        private async void PollTransporterInfoPerSecond(ITransporter transporter)
        {
            while (!_isPaused)
            {
                var tempSpeed = transporter.CompletedSize - _previousCompletedSize;
                _previousCompletedSize += tempSpeed;

                UpdateTransporterInfo(tempSpeed, transporter.CompletedSize / transporter.TotalSize);

                await Task.Delay(OneSecondDelay);
            }
        }

        private void UpdateTransporterInfo(DataSize speed, double progress)
        {
            //SynchronizationContext.Post(_ =>
            //{
            Speed = speed;
            Progress = progress;
            RaisePropertyChanged(nameof(CompletedSize));
            //}, null);
        }
    }
}
