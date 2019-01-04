using Accelerider.Windows.Infrastructure;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> callback)
        {
            Guards.ThrowIfNull(@this, callback);

            foreach (T item in @this)
            {
                callback(item);
            }
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> @this, Action<T> callback)
        {
            Guards.ThrowIfNull(@this, callback);

            foreach (var item in @this)
            {
                callback?.Invoke(item);
                yield return item;
            }
        }
    }
}
