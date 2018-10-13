using System;
using System.Collections.Generic;
using System.Reactive;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class ObserverList<T> : ObserverBase<T>
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();
        private readonly AsyncLocker _locker = new AsyncLocker();

        public void Add(IObserver<T> observer)
        {
            _locker.Await(() => _observers.Add(observer));
        }

        public void Remove(IObserver<T> observer)
        {
            _locker.Await(() => _observers.Remove(observer));
        }

        protected override void OnNextCore(T value)
        {
            _locker.Await(() => _observers.ForEach(item => item.OnNext(value)));
        }

        protected override void OnErrorCore(Exception error)
        {
            _locker.Await(() => _observers.ForEach(item => item.OnError(error)));
        }

        protected override void OnCompletedCore()
        {
            _locker.Await(() => _observers.ForEach(item => item.OnCompleted()));
        }
    }
}
