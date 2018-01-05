using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.TransportEngine
{
    public interface ITransporter
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

        bool ChangeTaskStatus(ITaskMetadata task, TransportTaskStatus status);

        IEnumerable<ITaskMetadata> FindAll();

        IEnumerable<ITaskMetadata> FindAll(TransportTaskStatus status);
    }

    public interface IDownloader : ITransporter
    {
        ITaskMetadata Add(DownloadTaskInfo taskInfo);

        IDownloader CreateChildDownloader();
    }

    public interface IUploader : ITransporter
    {
        ITaskMetadata Add(UploadTaskInfo taskInfo);

        IUploader CreateChildUploader();
    }
}
