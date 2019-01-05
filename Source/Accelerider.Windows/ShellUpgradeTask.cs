using System;
using Accelerider.Windows.Infrastructure.Upgrade;

namespace Accelerider.Windows
{
    public class ShellUpgradeTask : UpgradeTaskBase
    {
        public const string ShellName = "accelerider-shell-win";

        public ShellUpgradeTask()
            : base(ShellName, Environment.CurrentDirectory, "bin")
        {
        }

        protected override void OnCompleted(UpgradeInfo info, bool upgraded)
        {
            // TODO: Notify the user to restart.


        }

        protected override void OnError(Exception e)
        {
            // TODO: Logging
        }
    }
}
