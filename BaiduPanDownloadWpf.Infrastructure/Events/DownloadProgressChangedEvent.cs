using Prism.Events;

namespace BaiduPanDownloadWpf.Infrastructure.Events
{
    public class DownloadProgressChangedEvent : PubSubEvent<DownloadProgressChangedEventArgs>
    {
    }

    public class DownloadProgressChangedEventArgs : EventArgsBase
    {
        public long FileId { get; }
        public DataSize CurrentProgress { get; }
        public DataSize CurrentSpeed { get; }


        public DownloadProgressChangedEventArgs(long fileId, DataSize currentProgress, DataSize currentSpeed)
        {
            FileId = fileId;
            CurrentProgress = currentProgress;
            CurrentSpeed = currentSpeed;
        }
    }
}
