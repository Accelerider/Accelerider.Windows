using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadEngine : ITransportContainer
    {
        private IConfigureFile _progressFile;

        public ITransportContainer Initialize(IConfigureFile configFile)
        {
            _progressFile = configFile;
            return this;
        }

        public ITransportContainer Configure(Action<TransportSettings> settings)
        {
            throw new NotImplementedException();
        }

        public Task<IConfigureFile> ShutdownAsync()
        {
            throw new NotImplementedException();
        }

        public T Use<T>() where T : ITaskBuilder
        {
            if (typeof(T) != typeof(DownloadTaskBuilder))
                throw new TypeAccessException("Can not use this type");
            return (T) typeof(T).Assembly.CreateInstance(typeof(T).Name);

        }

        public IEnumerable<T> GetAllTasks<T>() where T : ITransportTask
        {
            throw new NotImplementedException();
        }
    }
}
