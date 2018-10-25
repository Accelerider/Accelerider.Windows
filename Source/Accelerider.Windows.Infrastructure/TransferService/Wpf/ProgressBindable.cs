using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure.TransferService.Wpf
{
    public class ProgressBindable : BindableBase
    {
        private readonly long _sampleIntervalBasedMilliseconds;

        private long _previousCompletedSize;
        private DataSize _completedSize;
        private DataSize _speed;
        private DataSize _totalSize;

        public DataSize CompletedSize
        {
            get => _completedSize;
            internal set { if (SetProperty(ref _completedSize, value)) OnCompletedSizeChanged(value); }
        }

        public DataSize Speed
        {
            get => _speed;
            private set => SetProperty(ref _speed, value);
        }

        public DataSize TotalSize
        {
            get => _totalSize;
            internal set => SetProperty(ref _totalSize, value);
        }

        internal ProgressBindable(long sampleIntervalBasedMilliseconds)
        {
            _sampleIntervalBasedMilliseconds = sampleIntervalBasedMilliseconds;
        }

        private void OnCompletedSizeChanged(DataSize value)
        {
            Speed = 1000 * (value - _previousCompletedSize) / _sampleIntervalBasedMilliseconds;
            _previousCompletedSize = value;
        }
    }
}
