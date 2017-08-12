using System;

namespace Accelerider.Windows.Events
{
    public class EventSubscription<TEventArgs>
    {
        private readonly DelegateReference _actionReference;
        private readonly DelegateReference _filterReference;

        public EventSubscription(Action<TEventArgs> action, Predicate<TEventArgs> filter)
        {
            _actionReference = new DelegateReference(action);
            _filterReference = new DelegateReference(filter);
        }

        public Action<TEventArgs> Action => (Action<TEventArgs>)_actionReference.Target;
        public Predicate<TEventArgs> Filter => (Predicate<TEventArgs>)_filterReference.Target;

        public Action<TEventArgs> GetExecutionStrategy()
        {
            var action = Action;
            var filter = Filter;
            if (action != null && filter != null)
            {
                return argument => { if (filter(argument)) action.Invoke(argument); };
            }
            return null;
        }
    }
}