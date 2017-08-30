using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal class TransferTaskToken : ITransferTaskToken
    {

        private volatile TransferStateEnum _transferState;
        private readonly TransferTaskBase _task;

        public TransferTaskToken(TransferTaskBase task)
        {
            _task = task;
            _task.TransferStateChanged += (sender, e) => OnTransferStateChanged(e);
        }

        public event EventHandler<TransferStateChangedEventArgs> TransferStateChanged;

        public TransferStateEnum TransferState { get => _transferState; private set => _transferState = value; }

        public IDiskFile FileInfo { get; set; }

        public DataSize Progress { get; set; }

        public async Task<bool> CancelAsync() => await _task.TryCancelAsync();

        public async Task<bool> PauseAsync() => await _task.TryPauseAsync();

        public async Task<bool> StartAsync(bool force = false) => await _task.TryRestartAsync();

        public bool Equals(ITransferTaskToken other) => FileInfo.FilePath == other?.FileInfo.FilePath;

        protected virtual void OnTransferStateChanged(TransferStateChangedEventArgs e)
        {
            TransferState = e.NewState;
            TransferStateChanged?.Invoke(this, e);
        }
    }
}
