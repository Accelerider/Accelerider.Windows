// ReSharper disable once CheckNamespace
namespace System
{
    public static class ObjectExtensions
    {
        private const double Tolerance = 1e-6;

        public static T CastTo<T>(this object value)
        {
            return typeof(T).IsValueType && value != null
                ? (T)Convert.ChangeType(value, typeof(T))
                : value is T typeValue ? typeValue : default;
        }

        public static bool EqualsWithinTolerance(this double @this, double other)
        {
            return Math.Abs(@this - other) < Tolerance;
        }

        public static bool GreaterOrEqual(this double @this, double other)
        {
            return @this > other || @this.EqualsWithinTolerance(other);
        }

        public static bool LessOrEqual(this double @this, double other)
        {
            return @this < other || @this.EqualsWithinTolerance(other);
        }
    }
}
