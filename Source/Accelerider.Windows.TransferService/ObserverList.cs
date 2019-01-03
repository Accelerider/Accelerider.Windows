using System;
using System.Collections.Generic;
using System.Reactive;

namespace Accelerider.Windows.TransferService
{
    internal class ObserverList<T> : ObserverBase<T>
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        public void Add(IObserver<T> observer) => _observers.Add(observer);

        public void Remove(IObserver<T> observer) => _observers.Remove(observer);

        protected override void OnNextCore(T value)
        {
            ForEachObserver(item => item.OnNext(value));
        }

        protected override void OnErrorCore(Exception error)
        {
            ForEachObserver(item => item.OnError(error));
        }

        protected override void OnCompletedCore()
        {
            ForEachObserver(item => item.OnCompleted());
        }

        private void ForEachObserver(Action<IObserver<T>> callback)
        {
            ForEach(_observers, callback);
        }

        private static void ForEach<TItem>(ICollection<TItem> list, Action<TItem> callback)
        {
            var backup = new TItem[list.Count];
            list.CopyTo(backup, 0);

            foreach (var item in backup)
            {
                callback?.Invoke(item);
            }
        }
    }
}
