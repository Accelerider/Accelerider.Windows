using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces.Files
{
    public interface ILocalDiskFile : IDiskFile
    {
        DateTime CompletedTime { get; }
    }
}
