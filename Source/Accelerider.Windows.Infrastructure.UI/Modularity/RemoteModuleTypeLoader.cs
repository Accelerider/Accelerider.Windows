using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Upgrade;
using Accelerider.Windows.TransferService;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure.Modularity
{
    public class RemoteModuleTypeLoader : IModuleTypeLoader, IDisposable
    {
        private readonly IAssemblyResolver _assemblyResolver;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is disposed of in the Dispose method.")]
        public RemoteModuleTypeLoader() : this(new AssemblyResolver()) { }

        public RemoteModuleTypeLoader(IAssemblyResolver assemblyResolver) => _assemblyResolver = assemblyResolver;

        #region Events

        /// <summary>
        /// Raised repeatedly to provide progress as modules are loaded in the background.
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        private void RaiseModuleDownloadProgressChanged(IModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive) => RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, bytesReceived, totalBytesToReceive));

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e) => ModuleDownloadProgressChanged?.Invoke(this, e);

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private void RaiseLoadModuleCompleted(IModuleInfo moduleInfo, Exception error) => RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));

        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e) => LoadModuleCompleted?.Invoke(this, e);

        #endregion

        /// <summary>
        /// Evaluates the <see cref="ModuleInfo.Ref"/> property to see if the current typeloader will be able to retrieve the <paramref name="moduleInfo"/>.
        /// Returns true if the <see cref="ModuleInfo.Ref"/> property starts with "file://", because this indicates that the file
        /// is a local file.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        /// <returns>
        /// 	<see langword="true"/> if the current typeloader is able to retrieve the module, otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="moduleInfo"/> is null.</exception>
        public bool CanLoadModuleType(IModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException(nameof(moduleInfo));

            return moduleInfo.Ref != null && moduleInfo.Ref.StartsWith(Uri.UriSchemeHttp, StringComparison.Ordinal);
        }

        /// <summary>
        /// Retrieves the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is rethrown as part of a completion event")]
        public async void LoadModuleType(IModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            if (!(moduleInfo is AppMetadata app))
                throw new ArgumentException($"the moduleInfo argument must be {nameof(AppMetadata)} type. ");

            try
            {
                var targetPath = Path.Combine(AcceleriderFolders.Apps, $"{app.ModuleName}-{app.Version.ToString(3)}");

                if (!Directory.Exists(targetPath))
                {
                    await DownloadModuleAsync(app, targetPath);
                }

                // 3. Load dll
                Directory
                    .GetFiles(targetPath, "*.dll", SearchOption.AllDirectories)
                    .ForEach(item => _assemblyResolver.LoadAssemblyFrom(item));

                // 4. Notify
                RaiseLoadModuleCompleted(moduleInfo, null);
            }
            catch (Exception ex)
            {
                RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }

        private async Task DownloadModuleAsync(IModuleInfo app, string targetPath)
        {
            var zipPath = $"{targetPath}.zip";

            // 1. Download the module
            var downloader = FileTransferService
                .GetDownloaderBuilder()
                .UseDefaultConfigure()
                .From(app.Ref)
                .To(zipPath)
                .Build();

            downloader
                .Sample(TimeSpan.FromMilliseconds(500))
                .Subscribe(item =>
                    RaiseModuleDownloadProgressChanged(app, downloader.GetCompletedSize(), downloader.GetTotalSize()));

            downloader.Run();

            await downloader;

            // 2. Unzip the module
            ZipFile.ExtractToDirectory(zipPath, targetPath);
        }


        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>.
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the associated <see cref="AssemblyResolver"/>.
        /// </summary>
        /// <param name="disposing">When <see langword="true"/>, it is being called from the Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            var disposableResolver = _assemblyResolver as IDisposable;
            disposableResolver?.Dispose();
        }

        #endregion
    }
}
