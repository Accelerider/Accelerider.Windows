using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Accelerider.Windows.Events
{
    public abstract class EventBase { }

    public abstract class EventBase<TEventArgs> : EventBase
    {
        private readonly List<EventSubscription<TEventArgs>> _subscriptions = new List<EventSubscription<TEventArgs>>();


        public void Subscribe(Action<TEventArgs> action, Predicate<TEventArgs> filter = null)
        {
            lock (_subscriptions)
            {
                _subscriptions.Add(new EventSubscription<TEventArgs>(action, filter ?? (e => true)));
            }
        }

        public void Unsubscribe(Action<TEventArgs> action)
        {
            lock (_subscriptions)
            {
                var eventSubscription = _subscriptions.FirstOrDefault(evt => evt.Action == action);
                if (eventSubscription != null)
                {
                    _subscriptions.Remove(eventSubscription);
                }
            }
        }

        public void Publish(TEventArgs e)
        {
            var executionStrategies = PruneAndReturnStrategies();
            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(e);
            }
        }


        private IEnumerable<Action<TEventArgs>> PruneAndReturnStrategies()
        {
            var returnList = new List<Action<TEventArgs>>();

            lock (_subscriptions)
            {
                for (var i = 0; i < _subscriptions.Count; i++)
                {
                    var listItem = _subscriptions[i].GetExecutionStrategy();

                    if (listItem == null)
                        _subscriptions.RemoveAt(i);
                    else
                        returnList.Add(listItem);
                }
            }
            return returnList;
        }
    }
}