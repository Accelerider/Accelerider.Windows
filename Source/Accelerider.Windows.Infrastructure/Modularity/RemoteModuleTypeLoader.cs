using System;
using System.IO;
using System.Linq;
using Accelerider.Windows.RemoteReference;
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

        private void RaiseModuleDownloadProgressChanged(ModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive) => RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, bytesReceived, totalBytesToReceive));

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e) => ModuleDownloadProgressChanged?.Invoke(this, e);

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error) => RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));

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
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException(nameof(moduleInfo));

            return moduleInfo.Ref != null && moduleInfo.Ref.StartsWith(Uri.UriSchemeFile, StringComparison.Ordinal);
        }

        /// <summary>
        /// Retrieves the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is rethrown as part of a completion event")]
        public async void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            if (!(moduleInfo is RemoteModuleInfo remoteModuleInfo))
                throw new ArgumentException($"The type of {nameof(moduleInfo)} must be {nameof(RemoteModuleInfo)}. ", nameof(moduleInfo));

            try
            {
                var missingRemoteRefs = remoteModuleInfo.RemoteRefs.Where(item => !File.Exists(item.LocalPath)).ToArray();
                if (missingRemoteRefs.Any())
                {
                    await new RemoteRefDownloader().StartAsync(
                        missingRemoteRefs,
                        new Progress<(long bytesReceived, long totalBytesToReceive)>(report =>
                            RaiseModuleDownloadProgressChanged(moduleInfo, report.bytesReceived, report.totalBytesToReceive)));
                    

                }

                remoteModuleInfo
                    .RemoteRefs
                    .Where(item => File.Exists(item.LocalPath))
                    .Select(item => item.LocalPath)
                    .ToList()
                    .ForEach(item => _assemblyResolver.LoadAssemblyFrom(item));

                RaiseLoadModuleCompleted(moduleInfo, null);
            }
            catch (Exception ex)
            {
                RaiseLoadModuleCompleted(moduleInfo, ex);
            }
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
