using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.TransportImpls
{
    internal class ConcurrentTaskQueue<T> : IEnumerable<T>
        where T : ITransportTask
    {
        private readonly List<T> _storage = new List<T>();

        public int Count => _storage.Count;

        public void Enqueue(T task)
        {
            lock (_storage)
            {
                _storage.Add(task);
            }
        }

        public T Dequeue(Func<T, bool> predicate = null)
        {
            lock (_storage)
            {
                var result = Peek(predicate);
                _storage.Remove(result);
                return result;
            }
        }

        public T Peek(Func<T, bool> predicate = null)
        {
            lock (_storage)
            {
                return predicate == null ? _storage.FirstOrDefault() : _storage.FirstOrDefault(predicate);
            }
        }

        public void Top(T task)
        {
            lock (_storage)
            {
                if (!_storage.Contains(task))
                    throw new ArgumentException();

                _storage.Remove(task);
                _storage.Insert(0, task);
            }
        }

        public bool Remove(T task)
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
        public IEnumerator<T> GetEnumerator() => _storage.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
