using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Exceptions
{
    public class InvalidDownloadStateChangeException : Exception
    {
        public DownloadStateEnum OldState { get; }
        public DownloadStateEnum NewState { get; }

        public InvalidDownloadStateChangeException(DownloadStateEnum oldState, DownloadStateEnum newState, string message) : base(message)
        {
            NewState = newState;
            OldState = oldState;
        }
    }
}
