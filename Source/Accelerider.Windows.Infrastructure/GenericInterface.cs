using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Accelerider.Windows.Infrastructure
{
    public interface IGenericInterface
    {
        Type Type { get; }

        Type[] GenericArguments { get; }

        TDelegate GetMethod<TDelegate>(string methodName, params Type[] argTypes);
    }

    public static class GenericInterfaceExtensions
    {
        public static IGenericInterface AsGenericInterface(this object @this, Type type)
        {
            var interfaceType = (
                    from @interface in @this.GetType().GetInterfaces()
                    where @interface.IsGenericType
                    let definition = @interface.GetGenericTypeDefinition()
                    where definition == type
                    select @interface
                )
                .SingleOrDefault();

            return interfaceType != null
                ? new GenericInterfaceImpl(@this, interfaceType)
                : null;
        }

        private class GenericInterfaceImpl : IGenericInterface
        {
            private static readonly Regex ActionDelegateRegex = new Regex(@"^System\.Action(`\d{1,2})?", RegexOptions.Compiled);
            private static readonly Regex FuncDelegateRegex = new Regex(@"^System\.Func`(\d{1,2})", RegexOptions.Compiled);

            private readonly object _instance;

            public Type Type { get; }

            public Type[] GenericArguments => Type.GetGenericArguments();

            public GenericInterfaceImpl(object instance, Type interfaceType)
            {
                _instance = instance;
                Type = interfaceType;
            }

            public TDelegate GetMethod<TDelegate>(string methodName, params Type[] argTypes)
            {
                switch (GetDelegateType<TDelegate>())
                {
                    case DelegateType.Action:
                        return GetAction<TDelegate>(methodName);
                    case DelegateType.ActionWithParams:
                        return GetActionWithParams<TDelegate>(methodName, argTypes);
                    default:
                        throw new NotSupportedException();
                }
            }

            private TDelegate GetActionWithParams<TDelegate>(string methodName, params Type[] argTypes)
            {
                var methodInfo = Type.GetMethod(methodName) ?? throw new ArgumentException(nameof(methodName));
                var argTypeList = argTypes.Any() ? argTypes : typeof(TDelegate).GetGenericArguments();
                (ParameterExpression expression, Type type)[] argObjectParameters = argTypeList
                    .Select(item => (Expression.Parameter(typeof(object)), item))
                    .ToArray();

                var method = Expression.Lambda<TDelegate>(
                        Expression.Call(
                            Expression.Constant(_instance),
                            methodInfo,
                            argObjectParameters.Select(item => Expression.Convert(item.expression, item.type))),
                        argObjectParameters.Select(item => item.expression))
                    .Compile();

                return method;
            }

            private TDelegate GetAction<TDelegate>(string methodName)
            {
                var methodInfo = Type.GetMethod(methodName) ?? throw new ArgumentException(nameof(methodName));
                var method = Expression.Lambda<TDelegate>(
                        Expression.Call(
                            Expression.Constant(_instance),
                            methodInfo))
                    .Compile();

                return method;
            }

            private static DelegateType GetDelegateType<TDelegate>()
            {
                var actionMatch = ActionDelegateRegex.Match(typeof(TDelegate).FullName ?? throw new InvalidOperationException());
                if (actionMatch.Success)
                {
                    return actionMatch.Groups.Count > 1 ? DelegateType.ActionWithParams : DelegateType.Action;
                }

                var funcMatch = FuncDelegateRegex.Match(typeof(TDelegate).FullName ?? throw new InvalidOperationException());
                if (funcMatch.Success)
                {
                    return int.Parse(actionMatch.Groups[1].Value) > 1 ? DelegateType.FuncWithParams : DelegateType.Func;
                }

                return DelegateType.NotSupported;
            }

            private enum DelegateType
            {
                NotSupported,
                Action,
                Func,
                ActionWithParams,
                FuncWithParams
            }
        }
    }
}
