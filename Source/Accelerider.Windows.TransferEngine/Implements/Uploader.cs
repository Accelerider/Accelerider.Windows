using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.TransferEngine.Implements
{
    internal class Uploader : TransfererBase, IUploader
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
