using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferContextBase
    {
        public Guid Id { get; } = Guid.NewGuid();

        public virtual long TotalSize { get; internal set; }

        public string LocalPath { get; internal set; }
    }

    public class TransferContext : TransferContextBase, INotifyPropertyChanged
    {
        private long _totalSize;

        public override long TotalSize
        {
            get => _totalSize;
            internal set
            {
                if (_totalSize == value) return;
                _totalSize = value;
                OnPropertyChanged();
            }
        }

        public IRemotePathProvider RemotePathProvider { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BlockTransferContext : TransferContextBase
    {
        private int _bytes;

        public long Offset { get; internal set; }

        public long CompletedSize { get; internal set; }

        [JsonIgnore]
        public int Bytes
        {
            get => _bytes;
            internal set
            {
                _bytes = value;
                CompletedSize += value;
            }
        }

        public string RemotePath { get; internal set; }
    }
}
