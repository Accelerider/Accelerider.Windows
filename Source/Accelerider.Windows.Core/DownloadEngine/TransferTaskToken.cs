using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal class TransferTaskToken : ITransferTaskToken
    {

        private volatile TransferTaskStatusEnum _transferTaskStatus;
        private readonly TransferTaskBase _task;

        public TransferTaskToken(TransferTaskBase task)
        {
            _task = task;
            _task.TransferTaskStatusChanged += (sender, e) => OnTransferTaskStatusChanged(e);
        }

        public event EventHandler<TransferTaskStatusChangedEventArgs> TransferTaskStatusChanged;

        public TransferTaskStatusEnum TransferTaskStatus { get => _transferTaskStatus; private set => _transferTaskStatus = value; }

        public string OwnerName
        {
            get { throw new NotImplementedException(); }
        }

        public IDiskFile FileInfo { get; set; }

        public DataSize Progress { get; set; }

        public async Task<bool> CancelAsync() => await _task.TryCancelAsync();

        public async Task<bool> PauseAsync() => await _task.TryPauseAsync();

        public async Task<bool> StartAsync(bool force = false) => await _task.TryRestartAsync();

        public bool Equals(ITransferTaskToken other) => FileInfo.FilePath == other?.FileInfo.FilePath;

        protected virtual void OnTransferTaskStatusChanged(TransferTaskStatusChangedEventArgs e)
        {
            TransferTaskStatus = e.NewStatus;
            TransferTaskStatusChanged?.Invoke(this, e);
        }
    }
}
