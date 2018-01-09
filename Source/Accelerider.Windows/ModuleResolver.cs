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
        private readonly string _moduleDirectory = $"{Environment.CurrentDirectory}/Modules";

        public ModuleResolver(IModuleCatalog moduleCatalog)
        {
            _moduleCatalog = moduleCatalog;
            ModuleConfigureFile = new ConfigureFile().Load($"{Environment.CurrentDirectory}/Accelerider.Modules.info");
        }

        public IConfigureFile ModuleConfigureFile { get; }

        public IModuleCatalog Initialize()
        {
            var modules = ModuleConfigureFile.GetValue<IList<ModuleInfo>>(ConstStrings.ModuleInfos);
            foreach (var module in modules.Reverse())
            {
                _moduleCatalog.AddModule(module);
            }

            return _moduleCatalog;
        }

        public void Save()
        {

        }

        private ModuleInfo ConvertAcceleriderModuleToModuleInfo(AcceleriderModule module)
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
