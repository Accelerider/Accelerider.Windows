using System;
using Prism.Mvvm;

namespace Accelerider.Windows.Infrastructure.WpfInteractions
{
    public class BindableProgress : BindableBase
    {
        private readonly long _sampleIntervalBasedMilliseconds;

        private long _previousCompletedSize;

        private DisplayDataSize _completedSize;
        private DisplayDataSize _speed;
        private DisplayDataSize _totalSize;
        private TimeSpan _remainingTime;

        public DisplayDataSize CompletedSize
        {
            get => _completedSize;
            internal set { if (SetProperty(ref _completedSize, value)) OnCompletedSizeChanged(value); }
        }

        public DisplayDataSize Speed
        {
            get => _speed;
            private set => SetProperty(ref _speed, value);
        }

        public DisplayDataSize TotalSize
        {
            get => _totalSize;
            internal set => SetProperty(ref _totalSize, value);
        }

        public TimeSpan RemainingTime
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        internal BindableProgress(long sampleIntervalBasedMilliseconds)
        {
            _sampleIntervalBasedMilliseconds = sampleIntervalBasedMilliseconds;
        }

        private void OnCompletedSizeChanged(DisplayDataSize value)
        {
            Speed = 1000.0 * (value - _previousCompletedSize) / _sampleIntervalBasedMilliseconds;
            RemainingTime = TimeSpan.FromSeconds(1.0 * (TotalSize - value) / Speed);
            _previousCompletedSize = value;
        }
    }
}
