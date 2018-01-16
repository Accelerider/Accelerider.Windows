using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Models;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;

namespace Accelerider.Windows
{
    internal class ModuleResolver
    {
        private static readonly MD5 Md5Algorithm = MD5.Create();
        private readonly UserMetadata _user;
        private readonly IAcceleriderApi _acceleriderApi;
        private readonly IModuleCatalog _moduleCatalog;
        private readonly IModuleManager _moduleManager;
        private readonly string _moduleDirectory = $"{Environment.CurrentDirectory}/Modules";

        public ModuleResolver(UserMetadata user, IAcceleriderApi acceleriderApi, IModuleCatalog moduleCatalog, IModuleManager moduleManager)
        {
            _user = user;
            _acceleriderApi = acceleriderApi;
            _moduleCatalog = moduleCatalog;
            _moduleManager = moduleManager;
        }

        public async Task LoadAsync()
        {
            var tasks = _user.ModuleIds.Select(id => _acceleriderApi.GetAppInfoByIdAsync(id));
            var moduleMetadatas = await Task.WhenAll(tasks);

            var abnormaolModules = FindAbnormalModules(moduleMetadatas);
            await DownloadModulesAsync(abnormaolModules);

            LoadModules(moduleMetadatas);
        }

        private IEnumerable<ModuleMetadata> FindAbnormalModules(IEnumerable<ModuleMetadata> modules)
        {
            var md5s = Directory.GetFiles(_moduleDirectory).Select(filePath => GetFileMd5(filePath)).ToArray();
            return modules.Where(module => md5s.Any(md5 => md5 == module.Checksum));
        }

        private async Task DownloadModulesAsync(IEnumerable<ModuleMetadata> abnormaolModules)
        {
            var modulePathTaskPairs = from module in abnormaolModules
                                      select new
                                      {
                                          Path = $"{_moduleDirectory}/{GetFileNameFromModuleType(module.ModuleType)}",
                                          Task = _acceleriderApi.GetAppByIdAsync(module.Id)
                                      };

            foreach (var item in modulePathTaskPairs)
            {
                if (File.Exists(item.Path)) File.Delete(item.Path);

                using (var fileStream = new FileStream(item.Path, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                {
                    var buffer = new byte[1024];
                    var offset = 0;
                    var stream = await item.Task;
                    while ((offset = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, offset);
                    }
                }
            }
        }

        private void LoadModules(IEnumerable<ModuleMetadata> modules)
        {
            foreach (var module in modules)
            {
                _moduleCatalog.AddModule(ConvertModuleMetadataToModuleInfo(module));
            }
            _moduleManager.Run();
        }


        private string GetFileMd5(string filePath)
        {
            var result = string.Empty;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                result = BitConverter.ToString(Md5Algorithm.ComputeHash(fileStream)).ToLower().Replace("-", string.Empty);
            }
            return result;
        }

        private ModuleInfo ConvertModuleMetadataToModuleInfo(ModuleMetadata module)
        {
            return new ModuleInfo
            {
                ModuleName = module.ModuleName,
                ModuleType = module.ModuleType,
                Ref = $"file://{_moduleDirectory}/{GetFileNameFromModuleType(module.ModuleType)}",
                InitializationMode = InitializationMode.OnDemand
            };
        }

        private string GetFileNameFromModuleType(string moduleType) => $"{moduleType.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]}.dll";
    }
}
