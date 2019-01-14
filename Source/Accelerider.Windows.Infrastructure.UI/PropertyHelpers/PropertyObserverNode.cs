using System.ComponentModel;
using static System.Linq.Expressions.Expression;

namespace System.Windows.Extensions.PropertyHelpers
{
    // Modified based on https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/PropertyObserverNode.cs
    internal class PropertyObserverNode
    {
        private readonly Action _action;

        private INotifyPropertyChanged _owner;
        private Func<object> _propertyGetter;

        internal object PropertyValue => _propertyGetter?.Invoke();

        internal string PropertyName { get; }

        internal PropertyObserverNode Next { get; set; }

        internal PropertyObserverNode(string propertyName, Action action)
        {
            PropertyName = propertyName;
            _action = () =>
            {
                if (Next != null)
                {
                    Next.UnsubscribeListener();
                    GenerateNextNode();
                }
                action?.Invoke();
            };
        }

        internal void SubscribeListenerFor(INotifyPropertyChanged inpcObject)
        {
            _owner = inpcObject;
            _owner.PropertyChanged += OnPropertyChanged;

            if (_propertyGetter == null)
                _propertyGetter = GetPropertyGetter<object>(_owner, PropertyName);

            if (Next != null) GenerateNextNode();
        }

        private void GenerateNextNode()
        {
            var nextProperty = _propertyGetter();
            if (nextProperty == null) return;

            if (!(nextProperty is INotifyPropertyChanged nextInpcObject))
                throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                                                    $"owns '{Next.PropertyName}' property, but the object does not implements INotifyPropertyChanged.");

            Next.SubscribeListenerFor(nextInpcObject);
        }

        private static Func<T> GetPropertyGetter<T>(object owner, string propertyName)
        {
            var propertyInfo = owner.GetType().GetProperty(propertyName);

            if (propertyInfo == null)
                throw new InvalidOperationException($"No the property named \"{propertyName}\" in the {owner.GetType()} type. ");

            var method = propertyInfo.GetGetMethod();

            return method.ReturnType != typeof(T) && method.ReturnType.IsValueType
                // Warning: Boxes the Value Type.
                ? Lambda<Func<T>>(Convert(Call(Constant(owner), method), typeof(T))).Compile()
                : (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), owner, propertyInfo.GetGetMethod());
        }

        private void UnsubscribeListener()
        {
            if (_owner != null)
                _owner.PropertyChanged -= OnPropertyChanged;

            _propertyGetter = null;

            Next?.UnsubscribeListener();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == PropertyName || e?.PropertyName == null)
            {
                _action?.Invoke();
            }
        }
    }
}
