using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure
{
    public sealed class ObservableSortedCollection<T> : BindableBase, ICollection, ICollection<T>, INotifyCollectionChanged
    {
        private const int InvalidIndex = -1;
        private readonly Comparison<T> _comparer;
        private readonly IList<T> _items = new List<T>();
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _items.Count;

        public object SyncRoot { get; } = new object();

        public bool IsSynchronized { get; } = true;

        public bool IsReadOnly { get; } = false;

        public ObservableSortedCollection(Comparison<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default.Compare;
        }

        public ObservableSortedCollection(IEnumerable<T> collection, Comparison<T> comparer) : this(comparer)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var list = collection.ToList();
            list.Sort(comparer);
            CopyFrom(list);
        }

        public void Update(T item)
        {
            if (Remove(item))
            {
                Add(item);
            }
        }

        public bool Contains(T item) => ExecuteSync(() => _items.Contains(item));

        public void Add(T item) => ExecuteSync(() =>
        {
            var index = GetAppropriateIndex(item);
            if (index == InvalidIndex) return;

            _items.Insert(index, item);
            RaisePropertyChanged(nameof(Count));
            //RaisePropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        });

        public void Clear() => ExecuteSync(() =>
        {
            _items.Clear();
            RaisePropertyChanged(nameof(Count));
            //RaisePropertyChanged("Item[]");
            OnCollectionReset();
        });

        public bool Remove(T item) => ExecuteSync(() =>
        {
            int index = _items.IndexOf(item);
            if (index < 0) return false;

            _items.RemoveAt(index);
            RaisePropertyChanged(nameof(Count));
            //RaisePropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            return true;
        });

        public IEnumerator<T> GetEnumerator() => ExecuteSync(() => _items.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(T[] array, int arrayIndex) => ExecuteSync(() => _items.CopyTo(array, arrayIndex));

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        private void ExecuteSync(Action action)
        {
            lock (SyncRoot)
            {
                action?.Invoke();
            }
        }

        private TReturn ExecuteSync<TReturn>(Func<TReturn> action)
        {
            lock (SyncRoot)
            {
                return action == null ? default(TReturn) : action();
            }
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            if (collection == null) return;

            foreach (T obj in collection)
            {
                _items.Add(obj);
            }
        }

        private int GetAppropriateIndex(T other)
        {
            if (other == null) return InvalidIndex;

            for (var i = 0; i < _items.Count; i++)
            {
                if (_comparer(_items[i], other) >= 0)
                {
                    return i;
                }
            }

            return _items.Count;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }
}
