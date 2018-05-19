using System;

namespace Accelerider.Windows.Infrastructure
{
    /// <summary>
    /// A data structure is used to represent the size, and can be converted to the appropriate size for display.
    /// </summary>
    public struct DataSize : IEquatable<DataSize>
    {
        public const int OneB = 1;
        public const int OneKB = 1024 * OneB;
        public const int OneMB = 1024 * OneKB;
        public const long OneGB = 1024 * OneMB;
        public const long OneTB = 1024 * OneGB;
        public const long OnePB = 1024 * OneTB;
        public const long OneEB = 1024 * OnePB;

        private SizeUnit _unit;

        public long BaseBValue { get; }

        public double Value { get; private set; }

        public SizeUnit Unit
        {
            get => _unit;
            set
            {
                if (value == _unit) return;

                Value *= Math.Pow(OneKB, _unit - value);
                _unit = value;
            }
        }

        private DataSize(long size)
        {
            BaseBValue = size;
            Value = size;
            _unit = SizeUnit.B;
            while (Value >= OneKB) // long.MaxValue = 8 EB. 
            {
                Value /= OneKB;
                _unit++;
            }
        }


        public override string ToString() => (Unit < SizeUnit.M ? $"{Value:N0} " : $"{Value:N2} ") + (Unit == SizeUnit.B ? $"{Unit}" : $"{Unit}B");

        public bool Equals(DataSize other) => this == other;

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is DataSize size && Equals(size);
        }

        public override int GetHashCode() => BaseBValue.GetHashCode();

        #region Operators

        public static DataSize operator +(DataSize left, DataSize right) => new DataSize(left.BaseBValue + right.BaseBValue);

        public static DataSize operator -(DataSize value) => new DataSize(-value.BaseBValue);

        public static DataSize operator -(DataSize left, DataSize right) => left + -right;

        public static double operator /(DataSize left, DataSize right) => 1.0 * left.BaseBValue / right.BaseBValue;

        public static bool operator ==(DataSize left, DataSize right) => left.BaseBValue == right.BaseBValue;

        public static bool operator !=(DataSize left, DataSize right) => !(left == right);

        public static bool operator >(DataSize left, DataSize right) => left.BaseBValue > right.BaseBValue;

        public static bool operator <(DataSize left, DataSize right) => left.BaseBValue < right.BaseBValue;

        public static implicit operator long(DataSize dataSize) => dataSize.BaseBValue;

        public static implicit operator DataSize(long @long) => new DataSize(@long);

        #endregion
    }
}