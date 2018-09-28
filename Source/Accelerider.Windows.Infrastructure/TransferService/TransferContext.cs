using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferContextBase
    {
        [JsonProperty]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [JsonProperty]
        public virtual long TotalSize { get; internal set; }

        [JsonProperty]
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

        [JsonProperty]
        public IRemotePathProvider RemotePathProvider { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BlockTransferContext : TransferContextBase
    {
        [JsonProperty]
        public long Offset { get; internal set; }

        [JsonProperty]
        public long CompletedSize { get; internal set; }

        [JsonProperty]
        public string RemotePath { get; internal set; }
    }
}
