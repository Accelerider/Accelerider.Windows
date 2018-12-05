using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.TransferService
{
    internal class ConcurrentList<T> : IReadOnlyCollection<T>
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

        public void Add(T item)
        {
            lock (_storage)
            {
                _storage.Add(item);
            }
        }

        public bool Remove(T item)
        {
            lock (_storage)
            {
                return _storage.Remove(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_storage)
            {
                if (!_storage.Contains(item)) return;
                _storage.Insert(index, item);
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

        public T Peek(Func<T, bool> predicate = null)
        {
            lock (_storage)
            {
                return predicate == null ? _storage.FirstOrDefault() : _storage.FirstOrDefault(predicate);
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
