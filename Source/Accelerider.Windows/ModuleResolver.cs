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
            _moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = "Group",
                ModuleType = "Accelerider.Windows.Modules.Group.GroupModule, Accelerider.Windows.Modules.Group, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                Ref = @"file://E:\VSTS\Accelerider\Source\Build\Modules\Accelerider.Windows.Modules.Group.dll",
                InitializationMode = InitializationMode.WhenAvailable
            });
            _moduleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = "NetDisk",
                ModuleType = "Accelerider.Windows.Modules.NetDisk.NetDiskModule, Accelerider.Windows.Modules.NetDisk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                Ref = @"file://E:\VSTS\Accelerider\Source\Build\Modules\Accelerider.Windows.Modules.NetDisk.dll",
                InitializationMode = InitializationMode.WhenAvailable
            });

            //var modules = ModuleConfigureFile.GetValue<IList<ModuleInfo>>(ConstStrings.ModuleInfos);

            //if (modules == null) return _moduleCatalog;

            //foreach (var module in modules.Reverse())
            //{
            //    _moduleCatalog.AddModule(module);
            //}

            return _moduleCatalog;
        }

        public void Save()
        {

        }


    }
}
