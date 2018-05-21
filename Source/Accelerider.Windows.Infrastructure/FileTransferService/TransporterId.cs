using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public sealed class TransporterId : IEquatable<TransporterId>
    {
        private readonly Guid _id = Guid.NewGuid();

        public bool Equals(TransporterId other) => !Equals(other, null) && _id.Equals(other._id);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || Equals(obj as TransporterId);

        public static bool operator ==(TransporterId left, TransporterId right)
        {
            if ((object)left == null || (object)right == null)
                return Equals(left, right);

            return left.Equals(right);
        }

        public static bool operator !=(TransporterId left, TransporterId right) => !(left == right);

        public override int GetHashCode() => _id.GetHashCode();

        public override string ToString() => _id.ToString();
    }
}
