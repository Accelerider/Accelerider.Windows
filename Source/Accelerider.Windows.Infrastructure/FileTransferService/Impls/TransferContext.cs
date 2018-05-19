using System;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferContext<T> where T : TransporterBaseImpl
    {
        private class Listener
        {
            private enum MoveDirection
            {
                TransportingToPending,
                TransportingToCompleted,
                Remove
            }

            private readonly TransferContext<T> _scheduler;

            public Listener(TransferContext<T> scheduler) => _scheduler = scheduler;

            public void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
            {
                var task = (T)sender;

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

        private readonly ConcurrentTaskQueue<T> _pendingQueue = new ConcurrentTaskQueue<T>();
        private readonly ConcurrentTaskQueue<T> _transportingQueue = new ConcurrentTaskQueue<T>();
        private readonly ConcurrentTaskQueue<T> _completedQueue = new ConcurrentTaskQueue<T>();

        private bool _isActived;
        private bool _isPromoting;

        public void Start()
        {
            _isActived = true;
            PromoteAsync();
        }

        public void Record(T task)
        {
            task.StatusChanged += new Listener(this).OnStatusChanged;
            switch (task.Status)
            {
                case TransferStatus.Ready:
                case TransferStatus.Suspended:
                case TransferStatus.Faulted:
                    _pendingQueue.Enqueue(task);
                    PromoteAsync();
                    break;
                case TransferStatus.Completed:
                    _completedQueue.Enqueue(task);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(task));
            }
        }

        public IEnumerable<T> GetAllTasks() => _pendingQueue.Union(_transportingQueue).Union(_completedQueue);

        public (IEnumerable<T> uncompletedTasks, IEnumerable<T> completedTasks) Shutdown()
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
