using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Prism.Logging;

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
        private readonly ILoggerFacade _logger;
        private readonly TransporterSettings _globalSettings = new TransporterSettings();
        private readonly TransferContext _downloaderContext;
        private readonly TransferContext _uploaderContext;
        private readonly HashSet<string> _registeredTransporterIds = new HashSet<string>();

        private IConfigureFile _configureFile;


        public TransferService(IUnityContainer container)
        {
            _container = container.CreateChildContainer();
            _logger = container.Resolve<ILoggerFacade>();

            _downloaderContext = new TransferContext();
            _uploaderContext = new TransferContext();

            _container.RegisterInstance(DownloaderContextKey, _downloaderContext);
            _container.RegisterInstance(UploaderContextKey, _uploaderContext);
        }


        public IEnumerable<IDownloader> Downloaders => _downloaderContext.GetAll().Cast<IDownloader>();

        public IEnumerable<IUploader> Uploaders => _uploaderContext.GetAll().Cast<IUploader>();

        public ITransferService Initialize(IConfigureFile configureFile = null)
        {
            _logger.Log($"Initializing {nameof(TransferService)} based on a " +
                        (configureFile == null ? string.Empty : "not") +
                        " null configure file. ", Category.Info, Priority.Low);

            _configureFile = configureFile ?? new ConfigureFile();

            _downloaderContext.Settings = _configureFile.GetValue<TransferContextSettings>(DownloaderContextSettings) ?? new TransferContextSettings();
            _uploaderContext.Settings = _configureFile.GetValue<TransferContextSettings>(UploaderContextSettings) ?? new TransferContextSettings();

            var uncompletedDownloaders = _configureFile.GetValue<List<Downloader>>(UncompletedDownloaders) ?? new List<Downloader>();
            _logger.Log($"Added {uncompletedDownloaders.Count} uncompleted downloader from configure.", Category.Info, Priority.Low);
            var completedDownloaders = _configureFile.GetValue<List<Downloader>>(CompletedDownloaders) ?? new List<Downloader>();
            _logger.Log($"Added {completedDownloaders.Count} completed downloader from configure.", Category.Info, Priority.Low);
            var uncompletedUploaders = _configureFile.GetValue<List<TransporterBase>>(UncompletedUploaders) ?? new List<TransporterBase>();
            _logger.Log($"Added {uncompletedUploaders.Count} uncompleted uploader from configure.", Category.Info, Priority.Low);
            var completedUploaders = _configureFile.GetValue<List<TransporterBase>>(CompletedUploaders) ?? new List<TransporterBase>();
            _logger.Log($"Added {completedUploaders.Count} completed uploader from configure.", Category.Info, Priority.Low);

            uncompletedDownloaders.Concat(completedDownloaders).ForEach(_downloaderContext.Add);
            _logger.Log("Successfully added all downloaders to the downloader context.", Category.Info, Priority.Low);
            uncompletedUploaders.Concat(completedUploaders).ForEach(_uploaderContext.Add);
            _logger.Log("Successfully added all uploaders to the uploader context.", Category.Info, Priority.Low);

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
            _logger.Log($"Running the {nameof(TransferService)}...", Category.Info, Priority.Low);
        }

        public IConfigureFile Shutdown()
        {
            var (uncompletedDownloaders, completedDownloaders) = _downloaderContext.Shutdown();
            _logger.Log($"Shutdown the downloader context, " +
                        $"which contains {uncompletedDownloaders.Count()} uncompleted downloader " +
                        $"and {completedDownloaders.Count()}", Category.Info, Priority.Low);
            //var (uncompletedUploaders, completedUploaders) = _uploaderContext.Shutdown();

            _configureFile.SetValue(DownloaderContextSettings, _downloaderContext.Settings);
            _configureFile.SetValue(UploaderContextSettings, _uploaderContext.Settings);
            _configureFile.SetValue(UncompletedDownloaders, (IEnumerable<Downloader>)uncompletedDownloaders);
            _configureFile.SetValue(CompletedDownloaders, (IEnumerable<Downloader>)completedDownloaders);
            //_configureFile.SetValue(UncompletedUploaders, (IEnumerable<TransporterBase>)uncompletedUploaders);
            //_configureFile.SetValue(CompletedUploaders, (IEnumerable<TransporterBase>)completedUploaders);

            _logger.Log($"Shutdown the {nameof(TransferService)}.", Category.Info, Priority.Low);
            return _configureFile;
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

            _logger.Log($"A transporter was disposed: Id = {transporter.Id}", Category.Info, Priority.Low);
        }
    }
}
