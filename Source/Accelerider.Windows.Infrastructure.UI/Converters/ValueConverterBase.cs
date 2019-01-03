using System;
using System.Globalization;
using System.Windows.Data;

namespace Accelerider.Windows.Infrastructure.Converters
{
    public abstract class ValueConverterBase<TSource, TTarget, TParameter> : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value.CastTo<TSource>(), parameter.CastTo<TParameter>());
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value.CastTo<TTarget>(), parameter.CastTo<TParameter>());
        }

        protected virtual TTarget ConvertNonNullValue(TSource value, TParameter parameter) => throw new NotSupportedException();

        protected virtual TTarget Convert(TSource value, TParameter parameter)
        {
            return value != null ? ConvertNonNullValue(value, parameter) : default;
        }

        protected virtual TSource ConvertBack(TTarget value, TParameter parameter) => throw new NotSupportedException();
    }

    public abstract class ValueConverterBase<TSource, TTarget> : ValueConverterBase<TSource, TTarget, object>
    {
        protected sealed override TTarget Convert(TSource value, object parameter) => Convert(value);

        protected sealed override TTarget ConvertNonNullValue(TSource value, object parameter) => throw new NotSupportedException();

        protected sealed override TSource ConvertBack(TTarget value, object parameter) => ConvertBack(value);

        protected virtual TTarget ConvertNonNullValue(TSource value) => throw new NotSupportedException();

        protected virtual TTarget Convert(TSource value) => value != null ? ConvertNonNullValue(value) : default;

        protected virtual TSource ConvertBack(TTarget value) => throw new NotSupportedException();
    }

}
