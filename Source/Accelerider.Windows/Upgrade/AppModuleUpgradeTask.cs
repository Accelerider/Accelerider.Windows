using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Upgrade;
using Prism.Modularity;

namespace Accelerider.Windows.Upgrade
{
    public class AppModuleUpgradeTask : UINotificationUpgradeTaskBase
    {
        private const string ModuleInfoFileName = "module-info.json";
        public const string ParameterCtorName = "name";

        private static readonly Regex ModuleFileRegex = new Regex(@"^Accelerider\.Windows\.Modules\.\w+?\.dll", RegexOptions.Compiled);

        private readonly IModuleManager _moduleManager;
        private readonly IModuleCatalog _moduleCatalog;

        public AppModuleUpgradeTask(string name, IModuleManager moduleManager, IModuleCatalog moduleCatalog)
            : base(name, AcceleriderFolders.Apps)
        {
            _moduleManager = moduleManager;
            _moduleCatalog = moduleCatalog;
        }

        public override async Task LoadFromLocalAsync()
        {
            var moduleVersion = GetMaxLocalVersion();
            if (_moduleCatalog.Exists(Name) || !TryExtractAppInfo(moduleVersion, out var moduleInfo)) return;

            _moduleCatalog.AddModule(moduleInfo);
            _moduleManager.LoadModule(moduleInfo.ModuleName);

            Logger.Info($"[LOAD] [{DateTime.Now}] {Name}-{moduleVersion}");

            // Delete old versions
            foreach ((Version version, string path) in GetLocalVersions())
            {
                if (version < moduleVersion)
                {
                    await path.TryDeleteAsync();
                }
            }
        }

        protected override async void OnDownloadCompleted(UpgradeInfo info)
        {
            base.OnDownloadCompleted(info);

            if (!_moduleCatalog.Exists(info.Name))
            {
                await LoadFromLocalAsync();
            }
            else
            {
                await ShowUpgradeNotificationDialogAsync(info);
            }
        }

        private bool TryExtractAppInfo(Version version, out IModuleInfo moduleInfo)
        {
            var installPath = GetInstallPath(version);
            var moduleInfoFilePath = Path.Combine(installPath, ModuleInfoFileName);

            if (File.Exists(moduleInfoFilePath))
            {
                moduleInfo = File.ReadAllText(moduleInfoFilePath).ToObject<ModuleInfo>();
                moduleInfo.Ref = GetModuleInfoRef(installPath);
                return true;
            }

            moduleInfo = null;
            return false;
        }

        private static string GetModuleInfoRef(string installPath)
        {
            var dllFilePath = Directory
                .GetFiles(installPath)
                .FirstOrDefault(item => ModuleFileRegex.IsMatch(Path.GetFileName(item)));

            return $"file:///{dllFilePath}";
        }
    }
}
