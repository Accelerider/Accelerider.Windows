using System;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferContext
    {
        private class Listener
        {
            private enum MoveDirection
            {
                TransportingToPending,
                TransportingToCompleted,
                Remove
            }

            private readonly TransferContext _scheduler;

            public Listener(TransferContext scheduler) => _scheduler = scheduler;

            public void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
            {
                var task = (TransporterBase)sender;

                switch (GetMoveDirection(e.OldStatus, e.NewStatus))
                {
                    case MoveDirection.TransportingToPending:
                        _scheduler._transportingQueue.Remove(task);
                        _scheduler._pendingQueue.Enqueue(task);
                        break;
                    case MoveDirection.TransportingToCompleted:
                        _scheduler._transportingQueue.Remove(task);
                        _scheduler._completedQueue.Enqueue(task);
                        break;
                    case MoveDirection.Remove:
                        task.StatusChanged -= OnStatusChanged;
                        var _ = _scheduler._pendingQueue.Remove(task) ||
                                _scheduler._completedQueue.Remove(task);
                        break;
                    default:
                        return;
                }

                _scheduler.PromoteAsync();
            }

            private MoveDirection GetMoveDirection(TransferStatus from, TransferStatus to)
            {
                if (from == TransferStatus.Transferring &&
                    (to == TransferStatus.Suspended ||
                     to == TransferStatus.Faulted))
                    return MoveDirection.TransportingToPending;

                if (from == TransferStatus.Transferring &&
                    to == TransferStatus.Completed)
                    return MoveDirection.TransportingToCompleted;

                if (from != TransferStatus.Transferring &&
                    to == TransferStatus.Disposed)
                    return MoveDirection.Remove;

                throw new ArgumentOutOfRangeException();
            }
        }

        private const int MaxParallelTaskCount = 4; // TODO: Move to configure file.

        private readonly ConcurrentTransporterQueue<TransporterBase> _pendingQueue = new ConcurrentTransporterQueue<TransporterBase>();
        private readonly ConcurrentTransporterQueue<TransporterBase> _transportingQueue = new ConcurrentTransporterQueue<TransporterBase>();
        private readonly ConcurrentTransporterQueue<TransporterBase> _completedQueue = new ConcurrentTransporterQueue<TransporterBase>();

        private bool _isActived;
        private bool _isPromoting;

        public void Add(TransporterBase transporter)
        {
            switch (transporter.Status)
            {
                case TransferStatus.Ready:
                case TransferStatus.Suspended:
                case TransferStatus.Faulted:
                    if (!_pendingQueue.Contains(transporter))
                    {
                        transporter.StatusChanged += new Listener(this).OnStatusChanged;
                        _pendingQueue.Enqueue(transporter);
                        PromoteAsync();
                    }
                    break;
                case TransferStatus.Completed:
                    if (!_pendingQueue.Contains(transporter))
                    {
                        transporter.StatusChanged += new Listener(this).OnStatusChanged;
                        _completedQueue.Enqueue(transporter);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transporter.Status));
            }
        }

        public void AsNext(TransporterBase transporter) => _pendingQueue.Top(transporter);

        public IEnumerable<ITransporter> GetAllTasks() => _pendingQueue.Union(_transportingQueue).Union(_completedQueue);

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
            if (!_isActived || _isPromoting || _transportingQueue.Count == MaxParallelTaskCount) return;

            _isPromoting = true;
            if (_transportingQueue.Count < MaxParallelTaskCount)
                IncreaseTransportingTaskAnync();
            else
                DecreaseTransportingTaskAnync();
            _isPromoting = false;
        }

        private void IncreaseTransportingTaskAnync()
        {
            while (_transportingQueue.Count < MaxParallelTaskCount)
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
            while (_transportingQueue.Count > MaxParallelTaskCount)
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
    }
}
