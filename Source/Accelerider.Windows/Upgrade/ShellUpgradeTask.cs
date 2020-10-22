using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Upgrade;

namespace Accelerider.Windows.Upgrade
{
    public class ShellUpgradeTask : UINotificationUpgradeTaskBase
    {
        public const string ShellName = "accelerider-shell";

        public ShellUpgradeTask()
            : base(ShellName, Environment.CurrentDirectory, "bin")
        {
        }

        public override Task LoadFromLocalAsync() => Task.CompletedTask;

        protected override async void OnDownloadCompleted(UpgradeInfo info)
        {
            base.OnDownloadCompleted(info);

            if (info.Version <= CurrentVersion) return;

            var launcherPath = Directory
                .GetFiles(GetInstallPath(info.Version))
                .SingleOrDefault(item => Path.GetFileName(item) == ProcessController.LauncherName);

            if (!string.IsNullOrEmpty(launcherPath))
            {
                try
                {
                    if (File.Exists(ProcessController.LauncherPath))
                    {
                        File.Delete(ProcessController.LauncherPath);
                    }
                    File.Move(launcherPath, ProcessController.LauncherPath);
                }
                catch (Exception e)
                {
                    Logger.Error($"[MOVE FILE] Move the {ProcessController.LauncherName} file failed. ", e);
                }
            }

            await ShowUpgradeNotificationDialogAsync(info);
        }
    }
}
