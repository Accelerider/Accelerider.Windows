using System;
using Prism.Mvvm;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.TransferService.WpfInteractions
{
    public class BindableProgress : BindableBase
    {
        private long _previousCompletedSize;
        private DateTimeOffset _previousTimestamp;

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
            private set => SetProperty(ref _remainingTime, value);
        }

        internal BindableProgress() { }

        private void OnCompletedSizeChanged(DisplayDataSize value)
        {
            var timestamp = DateTimeOffset.Now;

            Speed = 1.0 * (value - _previousCompletedSize) / (timestamp - _previousTimestamp).TotalSeconds;
            RemainingTime = Speed != 0
                ? TimeSpan.FromSeconds(1.0 * (TotalSize - value) / Speed.ValueBasedB)
                : TimeSpan.MaxValue;

            _previousTimestamp = timestamp;
            _previousCompletedSize = value;
        }
    }
}
