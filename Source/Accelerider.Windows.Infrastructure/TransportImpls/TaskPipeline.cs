using System;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.TransportImpls
{
    internal class TaskPipeline
    {
        private readonly Func<ITransportTask, TaskContext, ITransportTask> _handler;

        public TaskPipeline(Func<ITransportTask, TaskContext, ITransportTask> handler) => _handler = handler;

        public TaskPipeline Into(TaskPipeline pipeline) => new TaskPipeline((task, context) => pipeline._handler(_handler(task, context), context));

        public ITransportTask Process(ITransportTask task) => _handler(task, new TaskContext());
    }
}
