using System;
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

            if (CurrentVersion < info.Version)
            {
                await ShowUpgradeNotificationDialogAsync(info);
            }
        }
    }
}
