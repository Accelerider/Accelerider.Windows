using Accelerider.Windows.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace Accelerider.Windows.TransferEngine
{
    public interface ITransferer
    {
        // Life cycle management --------------------------------------------------------------------

        void Launch(string configFilePath);

        void Launch(IConfigureFile configFile);

        void Restart();

        void Shutdown();

        // Configure management ---------------------------------------------------------------------

        IConfigureFile Configure { get; }

        // Tasks management -------------------------------------------------------------------------

        bool Remove(ITaskMetadata task);

        bool ChangeTaskStatus(ITaskMetadata task, TransferTaskStatus status);

        IEnumerable<ITaskMetadata> FindAll();

        IEnumerable<ITaskMetadata> FindAll(TransferTaskStatus status);
    }

    public interface IDownloader : ITransferer
    {
        ITaskMetadata Add(DownloadTaskInfo taskInfo);

        IDownloader CreateChildDownloader();
    }

    public interface IUploader : ITransferer
    {
        ITaskMetadata Add(UploadTaskInfo taskInfo);

        IUploader CreateChildUploader();
    }
}
