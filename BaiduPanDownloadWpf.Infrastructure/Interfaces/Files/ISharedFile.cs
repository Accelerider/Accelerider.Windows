using System;
using System.Collections.Generic;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces.Files
{
    public interface ISharedFile : IFile
    {
        Uri ShareLink { get; }
        string Password { get; }
        DateTime SharedTime { get; }
        long VisitedNumber { get; }
        long DownloadedNumber { get; }
        long SavedNumber { get; }
    }
}
