using Accelerider.Windows.TransferService.WpfInteractions;

namespace Accelerider.Windows.Infrastructure.Converters
{
    public class NumberToDisplayDataSizeConverter : ValueConverterBase<double, DisplayDataSize>
    {
        protected override DisplayDataSize ConvertNonNullValue(double value) => value;
    }
}
