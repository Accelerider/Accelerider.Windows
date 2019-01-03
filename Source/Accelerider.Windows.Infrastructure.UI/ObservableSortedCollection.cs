using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Accelerider.Windows.Infrastructure
{
    public sealed class ObservableSortedCollection<T> : ObservableCollection<T>
    {
        private const int InvalidIndex = -1;

        private readonly Comparison<T> _comparer;

        public ObservableSortedCollection(Comparison<T> comparer = null)
        {
            _comparer = comparer ?? Comparer<T>.Default.Compare;
        }

        public ObservableSortedCollection(IEnumerable<T> collection, Comparison<T> comparer = null) : this(comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            var list = collection.ToList();
            list.Sort(_comparer);
            CopyFrom(list);
        }

        protected override void InsertItem(int index, T item)
        {
            index = GetAppropriateIndex(item);
            if (index == InvalidIndex) return;

            var previousIndex = Items.IndexOf(item);
            if (previousIndex == index) return;

            if (previousIndex != InvalidIndex)
                RemoveAt(previousIndex);

            base.InsertItem(index, item);
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            if (collection == null) return;

            foreach (T obj in collection)
            {
                Items.Add(obj);
            }
        }

        private int GetAppropriateIndex(T other)
        {
            if (other == null) return InvalidIndex;

            for (var i = 0; i < Items.Count; i++)
            {
                if (_comparer(Items[i], other) >= 0)
                {
                    return i;
                }
            }

            return Items.Count;
        }
    }
}
