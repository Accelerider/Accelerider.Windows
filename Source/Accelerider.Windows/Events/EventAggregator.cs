using System;
using System.Collections.Concurrent;

namespace Accelerider.Windows.Events
{
    public class EventAggregator
    {
        private readonly ConcurrentDictionary<Type, EventBase> _events = new ConcurrentDictionary<Type, EventBase>();

        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            return _events.ContainsKey(typeof(TEventType))
                ? (TEventType)_events[typeof(TEventType)]
                : (TEventType)(_events[typeof(TEventType)] = new TEventType());
        }
    }
}
