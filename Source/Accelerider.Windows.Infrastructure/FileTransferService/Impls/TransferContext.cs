using System;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferContext
    {
        private readonly ConcurrentTransporterQueue<TransporterBase> _pendingQueue = new ConcurrentTransporterQueue<TransporterBase>();
        private readonly ConcurrentTransporterQueue<TransporterBase> _transportingQueue = new ConcurrentTransporterQueue<TransporterBase>();
        private readonly ConcurrentTransporterQueue<TransporterBase> _completedQueue = new ConcurrentTransporterQueue<TransporterBase>();

        private bool _isActived;
        private bool _isPromoting;

        public TransferContextSettings Settings { get; set; }

        public void Add(TransporterBase transporter)
        {
            switch (transporter.Status)
            {
                case TransferStatus.Ready:
                case TransferStatus.Suspended:
                case TransferStatus.Faulted:
                    if (!_pendingQueue.Contains(transporter))
                    {
                        transporter.StatusChanged += OnStatusChanged;
                        _pendingQueue.Enqueue(transporter);
                        PromoteAsync();
                    }
                    break;
                case TransferStatus.Completed:
                    if (!_completedQueue.Contains(transporter))
                    {
                        transporter.StatusChanged += OnStatusChanged;
                        _completedQueue.Enqueue(transporter);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transporter.Status));
            }
        }

        public void AsNext(TransporterBase transporter)
        {
            if (!_pendingQueue.Contains(transporter)) return;

            if (transporter.Status != TransferStatus.Ready)
            {
                transporter.Status = TransferStatus.Ready;
            }

            _pendingQueue.Top(transporter);
        }

        public IEnumerable<ITransporter> GetAll() => _pendingQueue.Union(_transportingQueue).Union(_completedQueue);

        public void Run()
        {
            _isActived = true;
            PromoteAsync();
        }

        public (IEnumerable<ITransporter> uncompletedTasks, IEnumerable<ITransporter> completedTasks) Shutdown()
        {
            while (_transportingQueue.Any())
            {
                _transportingQueue.Dequeue().Suspend();
            }

            return (_pendingQueue, _completedQueue);
        }

        // -------------------------------------------------------------------------------------------------------------------------------------------

        private void PromoteAsync()
        {
            if (!_isActived || _isPromoting || _transportingQueue.Count == Settings.MaxParallelTranspoterCount) return;

            _isPromoting = true;
            if (_transportingQueue.Count < Settings.MaxParallelTranspoterCount)
                IncreaseTransportingTaskAnync();
            else
                DecreaseTransportingTaskAnync();
            _isPromoting = false;
        }

        private void IncreaseTransportingTaskAnync()
        {
            while (_transportingQueue.Count < Settings.MaxParallelTranspoterCount)
            {
                var pendingTask = _pendingQueue.Dequeue(task => task.Status == TransferStatus.Ready);
                if (pendingTask == null) return;

                try
                {
                    pendingTask.Start();
                    _transportingQueue.Enqueue(pendingTask);
                }
                catch
                {
                    PromoteAsync();
                }
            }
        }

        private void DecreaseTransportingTaskAnync()
        {
            while (_transportingQueue.Count > Settings.MaxParallelTranspoterCount)
            {
                var transportingTask = _transportingQueue.Dequeue();
                if (transportingTask == null) return;

                try
                {
                    transportingTask.Suspend();
                    _pendingQueue.Enqueue(transportingTask);
                }
                catch
                {
                    PromoteAsync();
                }
            }
        }

        public void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {
            var task = (TransporterBase)sender;

            if (e.OldStatus == TransferStatus.Transferring)
            {
                switch (e.NewStatus)
                {
                    case TransferStatus.Suspended:
                    case TransferStatus.Faulted:
                        _transportingQueue.Remove(task);
                        _pendingQueue.Enqueue(task);
                        break;
                    case TransferStatus.Completed:
                        _transportingQueue.Remove(task);
                        _completedQueue.Enqueue(task);
                        break;
                }
            }

            if (e.NewStatus == TransferStatus.Disposed)
            {
                task.StatusChanged -= OnStatusChanged;
                var _ = _pendingQueue.Remove(task) ||
                        _completedQueue.Remove(task) ||
                        _transportingQueue.Remove(task);
            }

            PromoteAsync();
        }
    }
}
