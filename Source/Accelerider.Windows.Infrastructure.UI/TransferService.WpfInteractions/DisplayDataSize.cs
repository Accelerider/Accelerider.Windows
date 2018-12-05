using System;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.TransferService.WpfInteractions
{
    public enum DataSizeUnit { B, KB, MB, GB, TB, PB, EB }

    public struct DisplayDataSize : IEquatable<DisplayDataSize>
    {
        public const int OneB = 1;
        public const int OneKB = 1024 * OneB;
        public const int OneMB = 1024 * OneKB;
        public const long OneGB = 1024 * OneMB;
        public const long OneTB = 1024 * OneGB;
        public const long OnePB = 1024 * OneTB;
        public const long OneEB = 1024 * OnePB;

        private DataSizeUnit _unit;

        public long Value { get; }

        public double DisplayValue { get; private set; }

        public DataSizeUnit Unit
        {
            get => _unit;
            set
            {
                if (value == _unit) return;

                DisplayValue *= Math.Pow(OneKB, _unit - value);
                _unit = value;
            }
        }

        public DisplayDataSize(double valueBasedB)
        {
            Value = (long)valueBasedB;
            DisplayValue = valueBasedB;
            _unit = DataSizeUnit.B;
            while (DisplayValue >= OneKB) // long.MaxValue = 8 EB. 
            {
                DisplayValue /= OneKB;
                _unit++;
            }
        }

        public override string ToString() => (Unit == DataSizeUnit.B ? $"{DisplayValue:N0} " : $"{DisplayValue:N2} ") + $"{Unit}";

        public static implicit operator long(DisplayDataSize dataSize) => dataSize.Value;

        public static implicit operator double(DisplayDataSize dataSize) => dataSize.DisplayValue;

        public static implicit operator string(DisplayDataSize dataSize) => dataSize.ToString();

        public static implicit operator DisplayDataSize(double value) => new DisplayDataSize(value);

        #region Implements Equals 

        public bool Equals(DisplayDataSize other) => Value.Equals(other.Value);

        public override bool Equals(object obj) => obj != null && obj is DisplayDataSize size && Equals(size);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(DisplayDataSize left, DisplayDataSize right) => left.Equals(right);

        public static bool operator !=(DisplayDataSize left, DisplayDataSize right) => !(left == right);

        #endregion
    }
}
