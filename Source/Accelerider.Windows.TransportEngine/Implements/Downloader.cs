using System;

namespace Accelerider.Windows.TransportEngine.Implements
{
    internal class Downloader : TransporterBase, IDownloader
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
