using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public sealed class TransporterToken : IEquatable<TransporterToken>
    {
        private readonly Guid _token = Guid.NewGuid();

        public bool Equals(TransporterToken other) => other != null && Equals(_token, other._token);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || Equals(obj as TransporterToken);

        public override int GetHashCode() => _token.GetHashCode();
    }
}
