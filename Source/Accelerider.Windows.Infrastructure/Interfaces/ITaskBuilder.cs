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

        ITransportTask Build();

        ITransportTask Update(ITransportTask task);
    }

    //public static class ITaskBuilderExtensions
    //{
    //    public static IDownloadTask Update(this ITaskBuilder<IDownloadTask> @this, IDownloadTask task)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static IUploadTask Update(this ITaskBuilder<IUploadTask> @this, IUploadTask task)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
