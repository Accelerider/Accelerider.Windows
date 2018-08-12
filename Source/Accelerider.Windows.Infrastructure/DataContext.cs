using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Accelerider.Windows.Infrastructure.Properties;

namespace Accelerider.Windows.Infrastructure
{
    public class DataContext
    {
        private readonly Dictionary<string, Delegate> _exportPropertyGetterDictionary = new Dictionary<string, Delegate>();
        private readonly Dictionary<string, List<Delegate>> _importPropertySetterDictionary = new Dictionary<string, List<Delegate>>();

        public object this[string key] => _exportPropertyGetterDictionary[key].DynamicInvoke();

        public void Export<T>(Expression<Func<T>> propertyExpression, string key = null)
        {
            key = key ?? PropertySupport.ExtractPropertyName(propertyExpression);

            if (_exportPropertyGetterDictionary.ContainsKey(key))
                throw new ArgumentException(string.Format(Resources.DataContext_ExportHasBeenExported_Exception, propertyExpression), nameof(propertyExpression));

            var getter = propertyExpression.Compile();

            _exportPropertyGetterDictionary[key] = getter;

            PropertyObserver.Observes(propertyExpression, () =>
            {
                if (!_importPropertySetterDictionary.TryGetValue(key, out var setterDelegates)) return;

                foreach (var setterDelegate in setterDelegates)
                {
                    var setter = (Action<T>)setterDelegate;
                    setter(getter());
                }
            });
        }

        public void Import<T>(Expression<Func<T>> propertyExpression, string key = null)
        {
            var parameter = Expression.Parameter(typeof(T));
            var setter = Expression.Lambda<Action<T>>(
                Expression.Assign(
                    propertyExpression.Body,
                    parameter),
                parameter).Compile();

            key = key ?? PropertySupport.ExtractPropertyName(propertyExpression);
            if (!_importPropertySetterDictionary.ContainsKey(key))
            {
                _importPropertySetterDictionary.Add(key, new List<Delegate>());
            }
            _importPropertySetterDictionary[key].Add(setter);
        }

        ///<summary>
        /// Provides support for extracting property information based on a property expression.
        ///</summary>
        public static class PropertySupport
        {
            /// <summary>
            /// Extracts the property name from a property expression.
            /// </summary>
            /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
            /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
            /// <returns>The name of the property.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
            ///     Not a <see cref="MemberExpression"/><br/>
            ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
            ///     Or, the property is static.
            /// </exception>
            public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
            {
                if (propertyExpression == null)
                    throw new ArgumentNullException(nameof(propertyExpression));

                return ExtractPropertyNameFromLambda(propertyExpression);
            }

            /// <summary>
            /// Extracts the property name from a LambdaExpression.
            /// </summary>
            /// <param name="expression">The LambdaExpression</param>
            /// <returns>The name of the property.</returns>
            /// <exception cref="ArgumentNullException">Thrown if the <paramref name="expression"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
            ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
            ///     Or, the property is static.
            /// </exception>
            internal static string ExtractPropertyNameFromLambda(LambdaExpression expression)
            {
                if (expression == null)
                    throw new ArgumentNullException(nameof(expression));

                var memberExpression = expression.Body as MemberExpression;
                if (memberExpression == null)
                    throw new ArgumentException(Resources.PropertySupport_NotMemberAccessExpression_Exception, nameof(expression));

                var property = memberExpression.Member as PropertyInfo;
                if (property == null)
                    throw new ArgumentException(Resources.PropertySupport_ExpressionNotProperty_Exception, nameof(expression));

                var getMethod = property.GetMethod;
                if (getMethod.IsStatic)
                    throw new ArgumentException(Resources.PropertySupport_StaticExpression_Exception, nameof(expression));

                return memberExpression.Member.Name;
            }
        }
    }
}
