using System;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface ITaskBuilderCreator
    {
        ITaskBuilder<IDownloadTask, Uri, FileLocation> CreateDownloadBuilder();

        ITaskBuilder<IUploadTask, FileLocation, Uri> CreateUploadBuilder();
    }
}
