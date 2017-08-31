using System;
using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.MockData
{
    public class TransferTaskTokenMockData : ITransferTaskToken
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private TransferTaskStatusEnum _transferTaskStatus;

        public TransferTaskTokenMockData(IDiskFile fileInfo)
        {
            FileInfo = fileInfo;
            TransferTaskStatus = TransferTaskStatusEnum.Transfering;
            ChangeProgress(_cancellationTokenSource.Token);
        }

        public event EventHandler<TransferTaskStatusChangedEventArgs> TransferTaskStatusChanged;

        public TransferTaskStatusEnum TransferTaskStatus
        {
            get => _transferTaskStatus;
            set
            {
                if (Equals(_transferTaskStatus, value)) return;
                OnTransferTaskStatusChanged(new TransferTaskStatusChangedEventArgs(this, _transferTaskStatus, _transferTaskStatus = value));
            }
        }

        public string OwnerName
        {
            get { throw new NotImplementedException(); }
        }

        public IDiskFile FileInfo { get; }
        public DataSize Progress { get; private set; } = new DataSize(0);

        public async Task<bool> PauseAsync()
        {
            _cancellationTokenSource.Cancel();
            TransferTaskStatus = TransferTaskStatusEnum.Paused;
            await Task.Delay(1000);
            return true;
        }

        public async Task<bool> StartAsync(bool force = false)
        {
            await Task.Delay(2000);

            _cancellationTokenSource = new CancellationTokenSource();
            Progress = new DataSize(0);
            ChangeProgress(_cancellationTokenSource.Token);

            TransferTaskStatus = TransferTaskStatusEnum.Transfering;
            return true;
        }

        public async Task<bool> CancelAsync()
        {
            if (!await PauseAsync()) return false;
            _cancellationTokenSource = null;
            Progress = new DataSize(0);
            TransferTaskStatus = TransferTaskStatusEnum.Canceled;
            return true;
        }

        public bool Equals(ITransferTaskToken other)
        {
            return FileInfo.FilePath == other?.FileInfo.FilePath;
        }

        private async void ChangeProgress(CancellationToken token)
        {
            var rand = new Random();
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(rand.Next(300, 1000));
                Progress += rand.Next(1, 1024 * 256);
                if (Progress < FileInfo.FileSize) continue;
                TransferTaskStatus = TransferTaskStatusEnum.Checking;
            }
        }

        protected virtual void OnTransferTaskStatusChanged(TransferTaskStatusChangedEventArgs e)
        {
            TransferTaskStatusChanged?.Invoke(this, e);
        }
    }
}