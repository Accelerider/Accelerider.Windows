using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferServiceImpl : ITransferService
    {
        private readonly IUnityContainer _container;
        private readonly TransporterSettings _settings = new TransporterSettings();

        private readonly TransferContext<DownloaderImpl> _downloadScheduler = new TransferContext<DownloaderImpl>();


        public TransferServiceImpl(IUnityContainer container)
        {
            _container = container.CreateChildContainer();
        }


        public ITransferService Initialize(IConfigureFile configFile)
        {
            // 1. restore tasks from configure file, which contain completed and uncompleted tasks. add them to task container.

            // 2. all uncompleted tasks will be suspended, rather than ready. 

            return this;
        }

        public ITransferService Configure(Action<TransporterSettings> settings)
        {
            settings?.Invoke(_settings);
            return this;
        }

        public IConfigureFile Shutdown()
        {
            // 1. all uncompleted tasks will be suspended.

            // 2. configure info and task info should be stored in a IConfigureFile instance and returned. 
            throw new NotImplementedException();
        }

        public T Use<T>() where T : ITransporterBuilder<ITransporter>
        {
            var builder = _container.Resolve<T>(
                new DependencyOverride<TransferContext<DownloaderImpl>>(_downloadScheduler));
            builder.Configure(_settings.CopyTo);
            return builder;
        }

        public IEnumerable<T> GetAll<T>() where T : ITransporter
        {
            throw new NotImplementedException();
        }

        public ITransferCommand Command(TransporterToken token)
        {
            throw new NotImplementedException();
        }
    }
}
