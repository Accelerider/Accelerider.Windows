using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class ConcurrentTaskQueue<T> : IEnumerable<T>
    {
        private readonly List<T> _storage = new List<T>();

        public int Count
        {
            get
            {
                lock (_storage)
                {
                    return _storage.Count;
                }
            }
        }

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
                var result = predicate == null ? _storage.FirstOrDefault() : _storage.FirstOrDefault(predicate);
                _storage.Remove(result);
                return result;
            }
        }

        public T Pop(Func<T, bool> predicate = null)
        {
            lock (_storage)
            {
                var result = predicate == null ? _storage.LastOrDefault() : _storage.LastOrDefault(predicate);
                _storage.Remove(result);
                return result;
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
        public IEnumerator<T> GetEnumerator()
        {
            lock (_storage)
            {
                var temp = new T[_storage.Count];
                _storage.CopyTo(temp, 0);
                return temp.Cast<T>().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
