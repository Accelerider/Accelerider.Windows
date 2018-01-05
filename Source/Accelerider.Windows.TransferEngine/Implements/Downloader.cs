using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.TransferEngine.Implements
{
    internal class Downloader : TransfererBase, IDownloader
    {
        public ITaskMetadata Add(DownloadTaskInfo taskInfo)
        {
            throw new NotImplementedException();
        }

        public IDownloader CreateChildDownloader()
        {
            throw new NotImplementedException();
        }
    }
}
