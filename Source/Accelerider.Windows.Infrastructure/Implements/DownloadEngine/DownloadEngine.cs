using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadEngine : ITransportEngine
    {
        private IConfigureFile _progressFile;

        public async Task InitializeAsync(string configFilePath)
        {
            await Task.Run(() =>
            {
                _progressFile = new ConfigureFile().Load(configFilePath);
            });
        }

        public Task InitializeAsync(IConfigureFile configFile)
        {
            _progressFile = configFile;
            return new Task(() => { });
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

        public IEnumerable<T> FindAll<T>() where T : ITransportTask
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll<T>(Func<T, bool> predicate) where T : ITransportTask
        {
            throw new NotImplementedException();
        }
    }
}
