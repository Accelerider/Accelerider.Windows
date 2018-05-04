using System;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface ITaskBuilder
    {
        ITaskBuilder From(string path);

        ITaskBuilder To(string path);

        ITaskBuilder Configure(Action<TransportSettings> settings);

        ITaskBuilder Configure(TransportSettings settings);

        ITaskBuilder Clone();
    }

    public interface IDownloadTaskBuilder : ITaskBuilder
    {
        IDownloadTask Build();

        IDownloadTask Update(IDownloadTask task);
    }

    public interface IUploadTaskBuilder : ITaskBuilder
    {
        IUploadTask Build();

        IUploadTask Update(IUploadTask task);
    }
}
