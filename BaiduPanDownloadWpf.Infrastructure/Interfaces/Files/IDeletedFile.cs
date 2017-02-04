using System;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces.Files
{
    public interface IDeletedFile : IDiskFile
    {
        DateTime DeletedTime { get; }

        int LeftDays { get; }
    }
}
