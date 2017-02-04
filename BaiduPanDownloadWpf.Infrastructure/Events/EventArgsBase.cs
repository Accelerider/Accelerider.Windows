using System;

namespace BaiduPanDownloadWpf.Infrastructure.Events
{
    public class EventArgsBase : EventArgs
    {
        public DateTime Timestamp { get; } = DateTime.Now;
    }
}
