using System;

namespace Accelerider.Windows.TransportEngine.Implements
{
    internal class Uploader : TransporterBase, IUploader
    {
        public ITaskMetadata Add(UploadTaskInfo taskInfo)
        {
            throw new NotImplementedException();
        }

        public IUploader CreateChildUploader()
        {
            throw new NotImplementedException();
        }
    }
}
