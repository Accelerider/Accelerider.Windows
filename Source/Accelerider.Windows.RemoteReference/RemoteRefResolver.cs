using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerider.Windows.RemoteReference
{
    public class RemoteRefResolver
    {
        public static readonly string ModuleBaseDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Accelerider.Windows", "Modules", "Base");

        public async Task<IEnumerable<string>> ResolveAsync(IList<RemoteRef> remoteRefs)
        {
            var globalMissingRemoteRefs = SearchMissingRemoteRefsInBaseFolder(GetMissingRemoteRefsInModuleFolder()).ToArray();

            if (globalMissingRemoteRefs.Any())
            {
                var downloadItems = globalMissingRemoteRefs.Select(item => new RemoteRef
                {
                    LocalPath = GetCacheDirectory(item.LocalPath),
                    RemotePath = item.RemotePath
                });

                await new RemoteRefDownloader().StartAsync(downloadItems.ToArray());
            }

            // Check if the missing assembly can be restored from module base path.
            IEnumerable<(string SourcePath, RemoteRef RemoteRef)> existingAssemblyCaches = (
                from remoteRef in GetMissingRemoteRefsInModuleFolder()
                let basePath = GetCacheDirectory(remoteRef.LocalPath)
                where Directory.Exists(basePath)
                select (basePath, remoteRef)
            );

            await Task.Run(() =>
            {
                // Copy existing assemblies from the base folder to the module folder.
                foreach (var item in existingAssemblyCaches)
                {
                    item.SourcePath.CreateHardLinkTo(item.RemoteRef.LocalPath);
                }
            });

            return remoteRefs
                .Where(item => !item.NonAssembly)
                .Where(item => Directory.Exists(item.LocalPath))
                .SelectMany(item => Directory.GetFiles(item.LocalPath, "*.dll", SearchOption.AllDirectories));

            // Gets the missing assembly from the module folder.
            IEnumerable<RemoteRef> GetMissingRemoteRefsInModuleFolder() =>
                remoteRefs.Where(item => !Directory.Exists(item.LocalPath));

            // Search the missing assembly from the base fodler.
            IEnumerable<RemoteRef> SearchMissingRemoteRefsInBaseFolder(IEnumerable<RemoteRef> missingRemoteRefs) =>
                missingRemoteRefs.Where(item => !Directory.Exists(GetCacheDirectory(item.LocalPath)));
        }

        // ReSharper disable once AssignNullToNotNullAttribute
        private static string GetCacheDirectory(string path) => Path.Combine(ModuleBaseDirectory, Path.GetFileName(path));

    }
};
