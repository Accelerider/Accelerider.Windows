using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Upgrade;
using Accelerider.Windows.ViewModels.Dialogs;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Prism.Modularity;

namespace Accelerider.Windows
{
    public class AppUpgradeTask : UpgradeTaskBase
    {
        public const string ParameterCtorName = "name";

        private static readonly Regex ModuleFileRegex = new Regex(@"^Accelerider\.Windows\.Modules\.\w+?\.dll", RegexOptions.Compiled);

        private readonly IModuleManager _moduleManager;
        private readonly IModuleCatalog _moduleCatalog;

        private bool _isUpgradeNotificationDialogOpened;

        public AppUpgradeTask(string name, IModuleManager moduleManager, IModuleCatalog moduleCatalog)
            : base(name, AcceleriderFolders.Apps)
        {
            _moduleManager = moduleManager;
            _moduleCatalog = moduleCatalog;
        }

        protected override async void OnCompleted(UpgradeInfo info, bool upgraded)
        {
            var moduleInfo = ConvertUpgradeInfoToModuleInfo(info);

            if (!_moduleCatalog.Exists(moduleInfo.ModuleName))
            {
                _moduleCatalog.AddModule(moduleInfo);
                _moduleManager.LoadModule(moduleInfo.ModuleName);
                Debug.WriteLine($"[LOAD] [{DateTime.Now}] {info.Name}-{info.Version}");

                // Delete old versions
                foreach ((Version version, string path) in GetLocalVersions())
                {
                    if (version < info.Version)
                        await DeleteDirectoryAsync(path);
                }
            }
            else if (upgraded)
            {
                // TODO: Notify the user to restart.
                await ShowUpgradeNotificationDialogAsync(info);
                Debug.WriteLine($"[RESTART] [{DateTime.Now}] {info.Name}-{info.Version}");
            }

            Debug.WriteLine($"[UPGRADE] [{DateTime.Now}] {info.Name}-{info.Version}");
        }

        protected override void OnError(Exception e)
        {
        }

        private async Task ShowUpgradeNotificationDialogAsync(UpgradeInfo info)
        {
            if (_isUpgradeNotificationDialogOpened) return;

            _isUpgradeNotificationDialogOpened = true;
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                while (true)
                {
                    try
                    {
                        var content = new UpgradeNotificationDialog();
                        if (content.DataContext is UpgradeNotificationDialogViewModel viewModel)
                        {
                            viewModel.Initialize(info);
                        }
                        await DialogHost.Show(content, "RootDialog");
                        return;
                    }
                    catch (InvalidOperationException)
                    {
                        await TimeSpan.FromSeconds(5);
                    }
                }
            });
            _isUpgradeNotificationDialogOpened = false;
        }

        private IModuleInfo ConvertUpgradeInfoToModuleInfo(UpgradeInfo info)
        {
            var appDirectory = Path.Combine(InstallDirectory, $"{Name}-{GetMaxLocalVersion().ToString(3)}");
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

        private static async Task DeleteDirectoryAsync(string path, int count = 0)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore
            }
            catch (IOException)
            {
                if (count < 10)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500 * count));
                    await DeleteDirectoryAsync(path, count + 1);
                }
            }
        }
    }
}
