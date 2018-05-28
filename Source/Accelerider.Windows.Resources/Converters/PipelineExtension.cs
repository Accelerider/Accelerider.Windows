using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Accelerider.Windows.Resources.Converters
{
    /*
     * e.g.: Visibility={Binding BooleanValue, Converter={converters:Pipeline {StaticResource NotConverter}, {StaticResource BooleanToVisibilityConverter}}}
     */

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class PipelineExtension : MarkupExtension
    {
        private ConverterCollection Converters { get; set; }

        #region Ctors

        /*
         * Have to list these constructor overloads here,
         * because the stupid xaml does not support the "params" keyword.
         * e.g.: ctor(params IValueConverter[] converters).
         */

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2) =>
            Converters = new ConverterCollection { converter1, converter2 };

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3)
            : this(converter1, converter2) => Converters.Add(converter3);

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3, IValueConverter converter4)
            : this(converter1, converter2, converter3) => Converters.Add(converter4);

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3, IValueConverter converter4, IValueConverter converter5)
            : this(converter1, converter2, converter3, converter4) => Converters.Add(converter5);

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider) => new ConverterAggregator(Converters);

        private class ConverterAggregator : IValueConverter
        {
            private readonly IEnumerable<IValueConverter> _converters;

            public ConverterAggregator(IEnumerable<IValueConverter> converters) => _converters = converters;

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
                _converters.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                _converters.Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, culture));
        }
    }

    public class ConverterCollection : Collection<IValueConverter>
    {
    }
}
