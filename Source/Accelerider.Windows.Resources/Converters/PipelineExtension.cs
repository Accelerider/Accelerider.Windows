using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Accelerider.Windows.Resources.Converters
{
    /*
     * e.g.: Visibility={Binding BooleanValue, Converter={converters:Pipeline {StaticResource NotConverter}, {StaticResource BooleanToVisibilityConverter}}}
     * But in ControlTemplate or DataTemplate, this syntax will throw an NullReferenceException, and must use the following syntax:
     *
     *    <Binding Path="Parent">
     *         <Binding.Converter>
     *             <converters:Pipeline>
     *                 <converters:Pipeline.Converters>
     *                     <converters:ConverterCollection>
     *                         <StaticResource ResourceKey="IsNullOperator"/>
     *                         <StaticResource ResourceKey="NotOperator" />
     *                         <StaticResource ResourceKey="BooleanToVisibilityConverter"/>
     *                     </converters:ConverterCollection>
     *                 </converters:Pipeline.Converters>
     *             </converters:Pipeline>
     *         </Binding.Converter>
     *     </Binding>
     */

    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class PipelineExtension : MarkupExtension, IValueConverter
    {
        [ConstructorArgument(nameof(Converters))]
        public ConverterCollection Converters { get; set; } = new ConverterCollection();

        #region Ctors

        /*
         * Have to list these constructor overloads here,
         * because the stupid xaml does not support the "params" keyword.
         * e.g.: ctor(params IValueConverter[] converters) cannot be used.
         */

        public PipelineExtension()
        {
        }

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2)
        {
            Converters.Add(converter1);
            Converters.Add(converter2);
        }

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3)
            : this(converter1, converter2) => Converters.Add(converter3);

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3, IValueConverter converter4)
            : this(converter1, converter2, converter3) => Converters.Add(converter4);

        public PipelineExtension(IValueConverter converter1, IValueConverter converter2, IValueConverter converter3, IValueConverter converter4, IValueConverter converter5)
            : this(converter1, converter2, converter3, converter4) => Converters.Add(converter5);

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Converters.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Converters.Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, culture));
    }

    public class ConverterCollection : Collection<IValueConverter>
    {
    }
}
