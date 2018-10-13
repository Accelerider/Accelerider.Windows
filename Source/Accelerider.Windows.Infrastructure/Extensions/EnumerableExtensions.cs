using Accelerider.Windows.Infrastructure;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> callback)
        {
            Guards.ThrowIfNull(callback);

            foreach (T item in @this)
            {
                callback(item);
            }
        }
    }
}
