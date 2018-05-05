using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public abstract class TaskBuilderBase : ITaskBuilder
    {
        protected TransportSettings Settings = new TransportSettings();


        public abstract ITaskBuilder From(string path);

        public abstract ITaskBuilder From(IEnumerable<string> paths);

        public abstract ITaskBuilder To(string path);

        public abstract ITaskBuilder To(IEnumerable<string> paths);

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
    }
}
