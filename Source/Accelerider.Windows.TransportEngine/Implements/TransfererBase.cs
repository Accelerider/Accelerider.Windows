using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.TransportEngine.Implements
{
    internal abstract class TransporterBase : ITransporter
    {
        public IConfigureFile Configure { get; }

        public bool ChangeTaskStatus(ITaskMetadata task, TransportTaskStatus status)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITaskMetadata> FindAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITaskMetadata> FindAll(TransportTaskStatus status)
        {
            throw new NotImplementedException();
        }

        public void Launch(string configFilePath)
        {
            throw new NotImplementedException();
        }

        public void Launch(IConfigureFile configFile)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ITaskMetadata task)
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
