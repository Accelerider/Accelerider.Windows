﻿using System;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.InfrastructureTests
{
    public class MockTransportTask : ITransporter
    {
        private TransferStatus _status;

        public bool Equals(ITransporter other)
        {
            return Equals(this, other);
        }


        public event EventHandler<TransferStatusChangedEventArgs> StatusChanged;

        public TransporterId Id { get; }

        public TransferStatus Status
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

        public FileLocator LocalPath { get; }

        public void Start()
        {
            Status = TransferStatus.Transferring;
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnStatusChanged(TransferStatus oldStatus, TransferStatus newStatus) =>
            StatusChanged?.Invoke(this, new TransferStatusChangedEventArgs(oldStatus, newStatus));
    }

    public class MockDownloadTask : MockTransportTask, IDownloader { }

    public class MockUploadTask : MockTransportTask, IUploader { }
}
