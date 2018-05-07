using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.TransportImpls
{
    internal class ConcurrentTaskQueue : IEnumerable<ITransportTask>
    {
        private readonly List<ITransportTask> _storage = new List<ITransportTask>();

        public int Count => _storage.Count;

        public void Enqueue(ITransportTask task)
        {
            lock (_storage)
            {
                _storage.Add(task);
            }
        }

        public ITransportTask Dequeue(Func<ITransportTask, bool> predicate = null)
        {
            lock (_storage)
            {
                var result = Peek(predicate);
                _storage.Remove(result);
                return result;
            }
        }

        public ITransportTask Peek(Func<ITransportTask, bool> predicate = null)
        {
            lock (_storage)
            {
                return predicate == null ? _storage.FirstOrDefault() : _storage.FirstOrDefault(predicate);
            }
        }

        public void Top(ITransportTask task)
        {
            lock (_storage)
            {
                if (!_storage.Contains(task))
                    throw new ArgumentException();

                _storage.Remove(task);
                _storage.Insert(0, task);
            }
        }

        public bool Remove(ITransportTask task)
        {
            lock (_storage)
            {
                return _storage.Remove(task);
            }
        }

        public void Clear()
        {
            lock (_storage)
            {
                _storage.Clear();
            }
        }

        #region Implements IEnumerable<T> interface
        public IEnumerator<ITransportTask> GetEnumerator() => _storage.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
