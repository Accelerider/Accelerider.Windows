using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public sealed class TransporterToken : IEquatable<TransporterToken>
    {
        private readonly Guid _token = Guid.NewGuid();

        public bool Equals(TransporterToken other) => !Equals(other, null) && _token.Equals(other._token);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || Equals(obj as TransporterToken);

        public static bool operator ==(TransporterToken left, TransporterToken right)
        {
            if ((object)left == null || (object)right == null)
                return Equals(left, right);

            return left.Equals(right);
        }

        public static bool operator !=(TransporterToken left, TransporterToken right) => !(left == right);

        public override int GetHashCode() => _token.GetHashCode();
    }
}
