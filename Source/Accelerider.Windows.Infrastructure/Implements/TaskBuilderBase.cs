using System;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class TaskBuilderBase : ITaskBuilder
    {
        protected TransportSettings Settings = new TransportSettings();


        public abstract ITaskBuilder From(string path);

        public abstract ITaskBuilder To(string path);

        public ITaskBuilder Configure(Action<TransportSettings> settings)
        {
            settings?.Invoke(Settings);
            return this;
        }

        public ITaskBuilder Configure(TransportSettings settings)
        {
            Settings = settings;
            return this;
        }

        public abstract ITaskBuilder Clone();

        public abstract ITransportTask Build();

        public abstract ITransportTask Update(ITransportTask task);
    }
}
