using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.InfrastructureTests
{
    public class MockTransportTask : ITransportTask
    {
        private TransportStatus _status;

        public bool Equals(ITransportTask other)
        {
            return Equals(this, other);
        }

        public event StatusChangedEventHandler StatusChanged;

        public bool IsDisposed { get; }

        public TransportStatus Status
        {
            get => _status;
            set
            {
                if (_status == value) return;

                var oldStatus = _status;
                _status = value;
                OnStatusChanged(oldStatus, _status);
            }
        }

        public DataSize CompletedSize { get; }

        public DataSize TotalSize { get; }

        public FileLocation LocalPath { get; }

        public Task StartAsync()
        {
            Status = TransportStatus.Transporting;
            return Task.CompletedTask;
        }

        public Task SuspendAsync()
        {
            throw new NotImplementedException();
        }

        public Task DisposeAsync()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnStatusChanged(TransportStatus oldStatus, TransportStatus newStatus) =>
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(oldStatus, newStatus));
    }

    public class MockDownloadTask : MockTransportTask, IDownloadTask { }

    public class MockUploadTask : MockTransportTask, IUploadTask { }
}
