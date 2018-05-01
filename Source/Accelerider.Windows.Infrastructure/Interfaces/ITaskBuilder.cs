using System;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface ITaskBuilder<out TTask, in TFromPath, in TToPath> where TTask : ITransportTask
    {
        ITaskBuilder<TTask, TFromPath, TToPath> From(TFromPath path);

        ITaskBuilder<TTask, TFromPath, TToPath> To(TToPath path);

        ITaskBuilder<TTask, TFromPath, TToPath> Configure(Action<TransportSettings> settings);

        ITaskBuilder<TTask, TFromPath, TToPath> Configure(TransportSettings settings);

        TTask Build();
    }
}
