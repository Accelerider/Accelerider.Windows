using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.DownloadEngine
{
    public class DownloadTaskToken
    {
        private volatile bool _isCancellationRequested;
        public bool IsCancellationRequested { get { return _isCancellationRequested; } }


    }
}
