using System;
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferServiceImpl : ITransferService
    {
        private readonly IUnityContainer _container;
        private readonly TransporterSettings _settings = new TransporterSettings();

        private readonly TransferContext _downloaderContext = new TransferContext();
        private readonly TransferContext _uploaderContext = new TransferContext();
        private readonly HashSet<string> _registeredTransporterIds = new HashSet<string>();

        public TransferServiceImpl(IUnityContainer container)
        {
            _container = container.CreateChildContainer();
        }


        public IEnumerable<IDownloader> Downloaders => _downloaderContext.GetAllTasks().Cast<IDownloader>();

        public IEnumerable<IUploader> Uploaders => _uploaderContext.GetAllTasks().Cast<IUploader>();

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

        public void Run()
        {
            _downloaderContext.Run();
        }

        public IConfigureFile Shutdown()
        {
            // 1. all uncompleted tasks will be suspended.

            // 2. configure info and task info should be stored in a IConfigureFile instance and returned. 
            throw new NotImplementedException();
        }

        public T Use<T>() where T : ITransporterBuilder<ITransporter>
        {
            var builder = _container.Resolve<T>();
            builder.Configure(_settings.CopyTo);
            return builder;
        }

        public ITransporterRegistry Register(ITransporter transporter)
        {
            var id = transporter.Id.ToString();

            if (_registeredTransporterIds.Contains(id))
                throw new InvalidOperationException($"Transporter with id {transporter.Id} is already registered and cannot be re-registered.");

            TransferContext context;
            switch (transporter)
            {
                case IDownloader _:
                    context = _downloaderContext;
                    break;
                case IUploader _:
                    context = _uploaderContext;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _registeredTransporterIds.Add(id);
            return new TransporterRegistryImpl((TransporterBaseImpl)transporter, context, () => _registeredTransporterIds.Remove(id));
        }
    }
}
