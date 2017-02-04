using Prism.Events;

namespace BaiduPanDownloadWpf.Infrastructure.Events
{
    public class DownloadStateChangedEvent : PubSubEvent<DownloadStateChangedEventArgs>
    {
    }

    public class DownloadStateChangedEventArgs : EventArgsBase
    {
        public long FileId { get; }
        public DownloadStateEnum OldState { get; }
        public DownloadStateEnum NewState { get; }

        public DownloadStateChangedEventArgs(long fileId, DownloadStateEnum oldState, DownloadStateEnum newState)
        {
            FileId = fileId;
            OldState = oldState;
            NewState = newState;
        }
    }
}
