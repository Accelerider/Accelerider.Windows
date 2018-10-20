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

        public long ValueBasedB { get; }

        public double Value { get; private set; }

        public DataSizeUnit Unit
        {
            get => _unit;
            set
            {
                if (value == _unit) return;

                Value *= Math.Pow(OneKB, _unit - value);
                _unit = value;
            }
        }

        public DisplayDataSize(double valueBasedB)
        {
            ValueBasedB = (long)valueBasedB;
            Value = valueBasedB;
            _unit = DataSizeUnit.B;
            while (Value >= OneKB) // long.MaxValue = 8 EB. 
            {
                Value /= OneKB;
                _unit++;
            }
        }

        public override string ToString() => (Unit == DataSizeUnit.B ? $"{Value:N0} " : $"{Value:N2} ") + $"{Unit}";

        public static implicit operator long(DisplayDataSize dataSize) => dataSize.ValueBasedB;

        public static implicit operator double(DisplayDataSize dataSize) => dataSize.Value;

        public static implicit operator string(DisplayDataSize dataSize) => dataSize.ToString();

        public static implicit operator DisplayDataSize(double @long) => new DisplayDataSize(@long);

        #region Implements Equals 

        public bool Equals(DisplayDataSize other) => ValueBasedB.Equals(other.ValueBasedB);

        public override bool Equals(object obj) => obj != null && obj is DisplayDataSize size && Equals(size);

        public override int GetHashCode() => ValueBasedB.GetHashCode();

        public static bool operator ==(DisplayDataSize left, DisplayDataSize right) => left.Equals(right);

        public static bool operator !=(DisplayDataSize left, DisplayDataSize right) => !(left == right);

        #endregion
    }
}
