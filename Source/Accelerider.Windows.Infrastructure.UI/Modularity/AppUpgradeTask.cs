using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Accelerider.Windows.Infrastructure.Upgrade;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure.Modularity
{
    public class AppUpgradeTask : UpgradeTaskBase
    {
        public const string ParameterCtorName = "name";

        private static readonly Regex ModuleFileRegex = new Regex(@"^Accelerider\.Windows\.Modules\.\w+?\.dll", RegexOptions.Compiled);

        private readonly IModuleManager _moduleManager;
        private readonly IModuleCatalog _moduleCatalog;

        public AppUpgradeTask(string name, IModuleManager moduleManager, IModuleCatalog moduleCatalog)
            : base(name, AcceleriderFolders.Apps)
        {
            _moduleManager = moduleManager;
            _moduleCatalog = moduleCatalog;
        }

        protected override void OnCompleted(UpgradeInfo info, bool upgraded)
        {
            var moduleInfo = ConvertUpgradeInfoToModuleInfo(info);

            if (!_moduleCatalog.Exists(moduleInfo.ModuleName))
            {
                _moduleCatalog.AddModule(moduleInfo);
                _moduleManager.LoadModule(moduleInfo.ModuleName);
                Debug.WriteLine($"[LOAD] [{DateTime.Now}] {info.Name}-{info.Version}");
            }
            else if (upgraded)
            {
                // TODO: Notify the user to restart.
                Debug.WriteLine($"[RESTART] [{DateTime.Now}] {info.Name}-{info.Version}");
            }

            Debug.WriteLine($"[UPGRADE] [{DateTime.Now}] {info.Name}-{info.Version}");
        }

        protected override void OnError(Exception e)
        {
        }

        private IModuleInfo ConvertUpgradeInfoToModuleInfo(UpgradeInfo info)
        {
            var appDirectory = Path.Combine(InstallDirectory, $"{Name}-{GetCurrentVersion().ToString(3)}");
            var appDllFilePath = Directory.GetFiles(appDirectory).FirstOrDefault(item => ModuleFileRegex.IsMatch(Path.GetFileName(item)));


            return new ModuleInfo
            {
                ModuleName = info.Name,
                Ref = $"file:///{appDllFilePath}",
                ModuleType = info.ModuleType,
                DependsOn = info.DependsOn != null
                    ? new Collection<string>(info.DependsOn.ToList())
                    : new Collection<string>()
            };
        }
    }
}
