namespace Accelerider.Windows.Infrastructure.Converters
{
    public class NotOperator : ValueConverterBase<bool, bool>
    {
        protected override bool Convert(bool value) => !value;

        protected override bool ConvertBack(bool value) => Convert(value);
    }

    public class DivideOperator : ValueConverterBase<double, double, double>
    {
        protected override double Convert(double value, double parameter) => value / parameter;

        protected override double ConvertBack(double value, double parameter) => value * parameter;
    }
}
