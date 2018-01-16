using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Core;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Models;

namespace Accelerider.Windows
{
    public class ModuleResolver
    {
        private readonly IModuleCatalog _moduleCatalog;
        private readonly IModuleManager _moduleManager;
        private readonly string _moduleDirectory = $"{Environment.CurrentDirectory}/Modules";

        public ModuleResolver(IModuleCatalog moduleCatalog, IModuleManager moduleManager)
        {
            _moduleCatalog = moduleCatalog;
            _moduleManager = moduleManager;
            ModuleConfigureFile = new ConfigureFile().Load($"{Environment.CurrentDirectory}/Accelerider.Modules.info");
        }

        public IConfigureFile ModuleConfigureFile { get; }

        public void LoadModules()
        {
            var modules = ModuleConfigureFile.GetValue<IList<ModuleInfo>>(ConstStrings.ModuleInfos);

            if (modules == null) return;

            foreach (var module in modules.Reverse())
            {
                _moduleCatalog.AddModule(module);
            }
            _moduleManager.Run();
        }

        public void UnloadModules()
        {
        }

        public void Save()
        {

        }

        private ModuleInfo ConvertAcceleriderModuleToModuleInfo(ModuleMetadata module)
        {
            return new ModuleInfo
            {
                ModuleName = module.ModuleName,
                ModuleType = module.ModuleType,
                Ref = $"file://{_moduleDirectory}/{module.ModuleType.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]}.dll",
                InitializationMode = InitializationMode.OnDemand
            };
        }

    }
}
