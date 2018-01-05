using Accelerider.Windows.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.TransferEngine.Implements
{
    internal abstract class TransfererBase : ITransferer
    {
        public IConfigureFile Configure { get; }

        public bool ChangeTaskStatus(ITaskMetadata task, TransferTaskStatus status)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITaskMetadata> FindAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITaskMetadata> FindAll(TransferTaskStatus status)
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
