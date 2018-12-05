

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key) where TValue : new()
        {
            if (!@this.ContainsKey(key))
            {
                @this[key] = new TValue();
            }

            return @this[key];
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            return @this.ContainsKey(key) ? @this[key] : default;
        }
    }
}
