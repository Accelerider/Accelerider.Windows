using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using MemberExpression = System.Linq.Expressions.MemberExpression;
using ConstantExpression = System.Linq.Expressions.ConstantExpression;

namespace System.Windows.Extensions.PropertyHelpers
{
    // Modified based on https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/PropertyObserver.cs
    public class PropertyObserver
    {
        public static PropertyObserver Observers(object owner, string expression, Action onChanged)
        {
            return new PropertyObserver(
                (INotifyPropertyChanged)owner,
                new Queue<string>(expression.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)),
                onChanged);
        }

        public static PropertyObserver Observes<T>(Expression<Func<T>> propertyExpression, Action onChanged)
        {
            return new PropertyObserver(propertyExpression.Body, onChanged);
        }

        private PropertyObserverNode _tailNode;

        public object PropertyValue => _tailNode.PropertyValue;

        private PropertyObserver(INotifyPropertyChanged owner, Queue<string> expressions, Action onChanged)
        {
            Initialize(owner, expressions, onChanged);
        }

        private PropertyObserver(Linq.Expressions.Expression propertyExpression, Action action)
        {
            Initialize(propertyExpression, action);
        }

        private void Initialize(INotifyPropertyChanged owner, Queue<string> expressions, Action onChanged)
        {
            var propObserverNodeRoot = new PropertyObserverNode(expressions.Dequeue(), onChanged);
            PropertyObserverNode previousNode = propObserverNodeRoot;
            foreach (var propName in expressions) // Create a node chain that corresponds to the property chain.
            {
                var currentNode = new PropertyObserverNode(propName, onChanged);
                previousNode.Next = currentNode;
                previousNode = currentNode;
            }

            _tailNode = previousNode;

            propObserverNodeRoot.SubscribeListenerFor(owner);
        }

        private static void Initialize(Linq.Expressions.Expression propertyExpression, Action action)
        {
            var propNameStack = new Stack<string>();
            while (propertyExpression is MemberExpression temp) // Gets the root of the property chain.
            {
                propertyExpression = temp.Expression;
                propNameStack.Push(temp.Member.Name); // Records the name of each property.
            }

            if (!(propertyExpression is ConstantExpression constantExpression))
                throw new NotSupportedException("Operation not supported for the given expression type. " +
                                                "Only MemberExpression and ConstantExpression are currently supported.");

            var propObserverNodeRoot = new PropertyObserverNode(propNameStack.Pop(), action);
            PropertyObserverNode previousNode = propObserverNodeRoot;
            foreach (var propName in propNameStack) // Create a node chain that corresponds to the property chain.
            {
                var currentNode = new PropertyObserverNode(propName, action);
                previousNode.Next = currentNode;
                previousNode = currentNode;
            }

            object propOwnerObject = constantExpression.Value;

            if (!(propOwnerObject is INotifyPropertyChanged inpcObject))
                throw new InvalidOperationException("Trying to subscribe PropertyChanged listener in object that " +
                                                    $"owns '{propObserverNodeRoot.PropertyName}' property, but the object does not implements INotifyPropertyChanged.");

            propObserverNodeRoot.SubscribeListenerFor(inpcObject);
        }
    }
}
