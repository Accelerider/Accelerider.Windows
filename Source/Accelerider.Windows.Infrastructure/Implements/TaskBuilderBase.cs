using System;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class TaskBuilderBase<TTask, TFromPath, TToPath> : ITaskBuilder<TTask, TFromPath, TToPath> where TTask : ITransportTask
    {
        protected TransportSettings Settings = new TransportSettings();


        public abstract ITaskBuilder<TTask, TFromPath, TToPath> From(TFromPath path);

        public abstract ITaskBuilder<TTask, TFromPath, TToPath> To(TToPath path);

        public ITaskBuilder<TTask, TFromPath, TToPath> Configure(Action<TransportSettings> settings)
        {
            settings?.Invoke(Settings);
            return this;
        }

        public ITaskBuilder<TTask, TFromPath, TToPath> Configure(TransportSettings settings)
        {
            Settings = settings;
            return this;
        }

        public abstract TTask Build();
    }
}
