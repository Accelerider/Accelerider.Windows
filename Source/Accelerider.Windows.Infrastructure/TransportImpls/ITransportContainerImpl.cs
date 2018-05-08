using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Infrastructure.TransportImpls
{
    internal class ITransportContainerImpl : ITransportContainer
    {
        private readonly IUnityContainer _container;
        private readonly TransportSettings _settings = new TransportSettings();

        private readonly TransportScheduler<IDownloadTask> _downloadScheduler = new TransportScheduler<IDownloadTask>();
        private readonly TransportScheduler<IUploadTask> _uploadScheduler = new TransportScheduler<IUploadTask>();


        public ITransportContainerImpl(IUnityContainer container)
        {
            _container = container.CreateChildContainer();
        }


        public ITransportContainer Initialize(IConfigureFile configFile)
        {
            // 1. restore tasks from configure file, which contain completed and uncompleted tasks. add them to task container.

            // 2. all uncompleted tasks will be suspended, rather than ready. 

            return this;
        }

        public ITransportContainer Configure(Action<TransportSettings> settings)
        {
            settings?.Invoke(_settings);
            return this;
        }

        public Task<IConfigureFile> ShutdownAsync()
        {
            // 1. all uncompleted tasks will be suspended.

            // 2. configure info and task info should be stored in a IConfigureFile instance and returned. 
            throw new NotImplementedException();
        }

        public T Use<T>() where T : ITaskBuilder
        {
            return _container.Resolve<T>(
                new DependencyOverride<TransportScheduler<IDownloadTask>>(_downloadScheduler),
                new DependencyOverride<TransportScheduler<IUploadTask>>(_uploadScheduler));
        }

        public IEnumerable<T> GetAllTasks<T>() where T : ITransportTask
        {
            throw new NotImplementedException();
        }

    }
}
