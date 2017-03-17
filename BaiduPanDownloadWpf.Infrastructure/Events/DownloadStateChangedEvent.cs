using System;
using System.Collections.Generic;
using System.Linq;
using BaiduPanDownloadWpf.Infrastructure.Exceptions;
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
#if DEBUG
            if (!CheckArgs(oldState, newState))
                throw new InvalidDownloadStateChangeException(oldState, newState, $"{oldState} can not be converted to {newState}.");
#endif

            FileId = fileId;
            OldState = oldState;
            NewState = newState;
        }

#if DEBUG
        private static readonly Dictionary<DownloadStateEnum, DownloadStateEnum> StateChangeRule = new Dictionary<DownloadStateEnum, DownloadStateEnum>
        {
            { DownloadStateEnum.Created, DownloadStateEnum.Waiting },
            { DownloadStateEnum.Waiting, DownloadStateEnum.Downloading | DownloadStateEnum.Canceled | DownloadStateEnum.Paused },
            { DownloadStateEnum.Downloading, DownloadStateEnum.Paused | DownloadStateEnum.Completed | DownloadStateEnum.Canceled | DownloadStateEnum.Faulted },
            { DownloadStateEnum.Paused, DownloadStateEnum.Downloading | DownloadStateEnum.Canceled | DownloadStateEnum.Paused }
        };
        private bool CheckArgs(DownloadStateEnum oldState, DownloadStateEnum newState)
        {
            if (oldState == DownloadStateEnum.Canceled ||
                oldState == DownloadStateEnum.Completed ||
                oldState == DownloadStateEnum.Faulted ||
                newState == DownloadStateEnum.Created) return false;

            return (StateChangeRule[oldState] & newState) == newState;
        }
#endif
    }
}
