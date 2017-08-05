using System;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.MockData
{
    public class TransferTaskTokenMockData : ITransferTaskToken
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _task;

        public TransferTaskTokenMockData(IDiskFile fileInfo)
        {
            FileInfo = fileInfo;
            _task = ChangeProgress(_cancellationTokenSource.Token);
        }

        public event EventHandler<TransferStateChangedEventArgs> TransferStateChanged;
        public TransferStateEnum TransferState { get; private set; } = TransferStateEnum.Waiting;
        public IDiskFile FileInfo { get; }
        public DataSize Progress { get; private set; } = new DataSize(0);

        public async Task<bool> PauseAsync()
        {
            _cancellationTokenSource.Cancel();
            TransferState = TransferStateEnum.Paused;
            await _task;
            return true;
        }

        public async Task<bool> RestartAsync()
        {
            await Task.Run(() =>
            {
                _cancellationTokenSource = new CancellationTokenSource();
                Progress = new DataSize(0);
                ChangeProgress(_cancellationTokenSource.Token);
            });
            TransferState = TransferStateEnum.Transfering;
            return true;
        }

        public async Task<bool> CancelAsync()
        {
            if (!await PauseAsync()) return false;
            _task = null;
            _cancellationTokenSource = null;
            Progress = new DataSize(0);
            TransferState = TransferStateEnum.Canceled;
            return true;
        }

        public bool Equals(ITransferTaskToken other)
        {
            return FileInfo.FilePath == other?.FileInfo.FilePath;
        }

        private async Task<int> ChangeProgress(CancellationToken token)
        {
            var rand = new Random();
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(rand.Next(300, 2000), token);
                Progress += rand.Next(1024 * 1024, 1024 * 1024 * 64);
                if (Progress < FileInfo.FileSize) continue;
                TransferState = TransferStateEnum.Completed;
                return 0;
            }
            return 0;
        }
    }
}