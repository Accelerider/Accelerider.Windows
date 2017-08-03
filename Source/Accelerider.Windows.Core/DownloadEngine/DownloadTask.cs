using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal class DownloadTask : TransferTaskBase
    {
        public override Task<bool> TryCancelAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> TryPauseAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> TryRestartAsync()
        {
            throw new NotImplementedException();
        }
    }
}
