using System;
using System.Reflection;

namespace Accelerider.Windows.Events
{
    public class DelegateReference
    {
        private readonly WeakReference _weakReference;
        private readonly MethodInfo _method;
        private readonly Type _delegateType;

        public DelegateReference(Delegate @delegate)
        {
            _weakReference = new WeakReference(@delegate.Target);
            _method = @delegate.GetMethodInfo();
            _delegateType = @delegate.GetType();
        }

        public Delegate Target => TryGetDelegate();

        private Delegate TryGetDelegate()
        {
            if (_method.IsStatic)
            {
                return _method.CreateDelegate(_delegateType, null);
            }
            var target = _weakReference.Target;
            return target != null ? _method.CreateDelegate(_delegateType, target) : null;
        }
    }
}