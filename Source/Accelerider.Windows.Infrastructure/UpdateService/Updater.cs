//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;
//using Accelerider.Windows.Infrastructure.FileTransferService;

//namespace Accelerider.Windows.Infrastructure.UpdateService
//{
//    public class UpdatedEventArgs : EventArgs
//    {
//        public UpdatedEventArgs(AppInfo app, bool success, string errorMessage)
//        {
//            App = app;
//            Success = success;
//            ErrorMessage = errorMessage;
//        }

//        public AppInfo App { get; }

//        public bool Success { get; }

//        public string ErrorMessage { get; }
//    }

//    public class Updater
//    {
//        private readonly Dictionary<TransporterId, (AppInfo App, IUnmanagedTransporterToken Token, IDownloader Downloader)> _downloaders = new Dictionary<TransporterId, (AppInfo App, IUnmanagedTransporterToken Token, IDownloader Downloader)>();
//        private readonly ITransferService _transferService;
//        private readonly List<AppInfo> _appInfos;

//        public event EventHandler<UpdatedEventArgs> Updated;

//        public FileLocator AppDirectory { get; set; }

//        public Func<string, Version, string> AppAddressProvider { get; set; }

//        public Updater(ITransferService transferService)
//        {
//            _transferService = transferService;
//            _appInfos = new List<AppInfo>();
//        }

//        public void Add(AppInfo appInfo) => _appInfos.Add(appInfo);

//        public void Add(IEnumerable<AppInfo> appInfos) => _appInfos.AddRange(appInfos);

//        public bool Remove(AppInfo appInfo) => _appInfos.Remove(appInfo);

//        /// <summary>
//        /// Check if managed apps have available updates.
//        /// </summary>
//        /// <returns>Retures a collection of <see cref="AppInfo"/> that can be updated.</returns>
//        public Task<IReadOnlyCollection<SpecifiedVersionAppInfo>> GetAvailableUpdatesAsync() => Task.Run<IReadOnlyCollection<SpecifiedVersionAppInfo>>(() =>
//        {
//            // 1. flatten the _appInfos;
//            var appInfos = (from rootAppInfo in _appInfos
//                            from appInfo in GetDependencies(rootAppInfo)
//                            select appInfo).ToList();
//            // 2. compare these appInfo with local file.
//            try
//            {
//                List<(string AssemblyName, Version Version)> assemblyNameInfos =
//                    (from fileInfo in new DirectoryInfo(AppDirectory).GetFiles("*.dll", SearchOption.AllDirectories)
//                     let assemblyName = Assembly.LoadFile(fileInfo.FullName).GetName()
//                     select (assemblyName.Name, assemblyName.Version)).ToList();

//                foreach (var info in assemblyNameInfos)
//                {
//                    Debug.WriteLine($"{info.AssemblyName} - {info.Version}");
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }

//            throw new NotImplementedException();
//        });

//        /// <summary>
//        /// Update all apps that can be updated to the latest version.
//        /// </summary>
//        /// <returns>Returns a tuple that represent the result of this update.</returns>
//        public async Task UpdateAsync()
//        {
//            var avaliableUpdates = await GetAvailableUpdatesAsync();
//            List<(AppInfo App, IDownloader Downloader)> downloaders = avaliableUpdates.Select(item => (item, CreateDownloader(item))).ToList();
//            downloaders.ForEach(item =>
//            {
//                item.Downloader.StatusChanged += OnStatusChanged;
//                var unmanagedToken = _transferService.Register(item.Downloader).AsUnmanaged();
//                _downloaders[item.Downloader.Id] = (item.App, unmanagedToken, item.Downloader);
//                unmanagedToken.Start();
//            });
//        }

//        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
//        {
//            if (e.NewStatus != TransferStatus.Completed &&
//                e.NewStatus != TransferStatus.Faulted ||
//                !(sender is IDownloader downloader)) return;

//            if (_downloaders.TryGetValue(downloader.Id, out var value))
//            {
//                using (value.Token)
//                {
//                    _downloaders.Remove(downloader.Id);
//                    Updated?.Invoke(this, new UpdatedEventArgs(value.App, downloader.Status == TransferStatus.Completed, string.Empty));
//                }
//            }
//            ((IDownloader)sender).StatusChanged -= OnStatusChanged;
//        }

//        private IDownloader CreateDownloader(SpecifiedVersionAppInfo appInfo) => _transferService
//            .UseDefaultDownloaderBuilder()
//            .Configure(settings =>
//            {
//                settings.MaxErrorCount = 3;
//                settings.AutoSwitchUri = true;
//                settings.BlockSize = DataSize.OneMB * 50;
//                settings.ThreadCount = 16;
//            })
//            .From(AppAddressProvider(appInfo.Name, appInfo.Version))
//            .To(AppDirectory)
//            .Build();

//        private IEnumerable<DependencyAppInfo> GetDependencies(AppInfo appInfo)
//        {
//            var accumulator = new List<DependencyAppInfo>();
//            GetDependencies(appInfo, accumulator);
//            return accumulator.AsReadOnly();
//        }

//        private void GetDependencies(AppInfo appInfo, ICollection<DependencyAppInfo> accumulator)
//        {
//            if (accumulator == null) throw new ArgumentNullException(nameof(accumulator));

//            foreach (var info in appInfo.Dependencies)
//            {
//                if (accumulator.All(item => item.Checksum != info.Checksum))
//                {
//                    accumulator.Add(info);
//                    GetDependencies(info, accumulator);
//                }
//            }
//        }
//    }
//}
