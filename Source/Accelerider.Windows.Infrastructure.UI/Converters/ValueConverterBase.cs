using System;
using System.Globalization;
using System.Windows.Data;

namespace Accelerider.Windows.Infrastructure.Converters
{
    public abstract class ValueConverterBase<TSource, TTarget, TParameter> : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null && default(TSource) == null
                // ReSharper disable once ExpressionIsAlwaysNull
                ? Convert((TSource)value, (TParameter)parameter)
                : value is TSource sourceValue
                    ? Convert(sourceValue, (TParameter)parameter)
                    : default;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null && default(TTarget) == null
                // ReSharper disable once ExpressionIsAlwaysNull
                ? ConvertBack((TTarget)value, (TParameter)parameter)
                : value is TTarget sourceValue
                    ? ConvertBack(sourceValue, (TParameter)parameter)
                    : default;
        }

        protected abstract TTarget Convert(TSource value, TParameter parameter);

        protected virtual TSource ConvertBack(TTarget value, TParameter parameter) => throw new NotSupportedException();
    }

    public abstract class ValueConverterBase<TSource, TTarget> : ValueConverterBase<TSource, TTarget, object>
    {
        protected sealed override TTarget Convert(TSource value, object parameter) => Convert(value);

        protected sealed override TSource ConvertBack(TTarget value, object parameter) => ConvertBack(value);

        protected abstract TTarget Convert(TSource value);

        protected virtual TSource ConvertBack(TTarget value) => throw new NotSupportedException();
    }
}
