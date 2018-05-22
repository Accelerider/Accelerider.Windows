using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Infrastructure.FileTransferService.Impls
{
    internal class TransferService : ITransferService
    {
        #region Configure Keys

        public const string DownloaderContextSettings = nameof(DownloaderContextSettings);
        public const string UploaderContextSettings = nameof(UploaderContextSettings);
        public const string UncompletedDownloaders = nameof(UncompletedDownloaders);
        public const string UncompletedUploaders = nameof(UncompletedUploaders);
        public const string CompletedDownloaders = nameof(CompletedDownloaders);
        public const string CompletedUploaders = nameof(CompletedUploaders);

        #endregion

        public const string DownloaderContextKey = "DownloaderContext";
        public const string UploaderContextKey = "UploaderContext";


        private readonly IUnityContainer _container;
        private readonly TransporterSettings _globalSettings = new TransporterSettings();

        private readonly TransferContext _downloaderContext;
        private readonly TransferContext _uploaderContext;
        private readonly HashSet<string> _registeredTransporterIds = new HashSet<string>();


        public TransferService(IUnityContainer container)
        {
            _container = container.CreateChildContainer();

            _downloaderContext = new TransferContext();
            _uploaderContext = new TransferContext();

            _container.RegisterInstance(DownloaderContextKey, _downloaderContext);
            _container.RegisterInstance(UploaderContextKey, _uploaderContext);
        }


        public IEnumerable<IDownloader> Downloaders => _downloaderContext.GetAll().Cast<IDownloader>();

        public IEnumerable<IUploader> Uploaders => _uploaderContext.GetAll().Cast<IUploader>();

        public ITransferService Initialize(IConfigureFile configFile)
        {
            _downloaderContext.Settings = configFile.GetValue<TransferContextSettings>(DownloaderContextSettings);
            _uploaderContext.Settings = configFile.GetValue<TransferContextSettings>(UploaderContextSettings);

            var uncompletedDownloaders = configFile.GetValue<List<Downloader>>(UncompletedDownloaders);
            var completedDownloaders = configFile.GetValue<List<Downloader>>(CompletedDownloaders);
            var uncompletedUploaders = configFile.GetValue<List<TransporterBase>>(UncompletedUploaders);
            var completedUploaders = configFile.GetValue<List<TransporterBase>>(CompletedUploaders);

            uncompletedDownloaders.Concat(completedDownloaders).ForEach(_downloaderContext.Add);
            uncompletedUploaders.Concat(completedUploaders).ForEach(_uploaderContext.Add);

            return this;
        }

        public ITransferService Configure(Action<TransporterSettings> settings)
        {
            settings?.Invoke(_globalSettings);
            return this;
        }

        public void Run()
        {
            Parallel.Invoke(_downloaderContext.Run, _uploaderContext.Run);
        }

        public IConfigureFile Shutdown()
        {
            var (uncompletedDownloaders, completedDownloaders) = _downloaderContext.Shutdown();
            var (uncompletedUploaders, completedUploaders) = _uploaderContext.Shutdown();

            var result = new ConfigureFile();
            result.SetValue(DownloaderContextSettings, _downloaderContext.Settings);
            result.SetValue(UploaderContextSettings, _uploaderContext.Settings);
            result.SetValue(UncompletedDownloaders, uncompletedDownloaders);
            result.SetValue(CompletedDownloaders, completedDownloaders);
            result.SetValue(UncompletedUploaders, uncompletedUploaders);
            result.SetValue(CompletedUploaders, completedUploaders);

            return result;
        }

        public T Use<T>() where T : ITransporterBuilder<ITransporter>
        {
            var builder = _container.Resolve<T>();
            builder.Configure(_globalSettings.CopyTo);
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
            transporter.StatusChanged += OnTransporterDisposed;
            return new TransporterRegistry((TransporterBase)transporter, context);
        }

        private void OnTransporterDisposed(object sender, TransferStatusChangedEventArgs e)
        {
            if (e.NewStatus != TransferStatus.Disposed) return;

            var transporter = (ITransporter)sender;
            _registeredTransporterIds.Remove(transporter.Id.ToString());
            transporter.StatusChanged -= OnTransporterDisposed;
        }
    }
}
