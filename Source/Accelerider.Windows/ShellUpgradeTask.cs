using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Accelerider.Windows.Infrastructure.Upgrade;
using Accelerider.Windows.ViewModels.Dialogs;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows
{
    public class ShellUpgradeTask : UpgradeTaskBase
    {
        public const string ShellName = "accelerider-shell-win";

        private bool _isUpgradeNotificationDialogOpened;

        public ShellUpgradeTask()
            : base(ShellName, Environment.CurrentDirectory, "bin")
        {
        }

        protected override async void OnCompleted(UpgradeInfo info, bool upgraded)
        {
            // TODO: Notify the user to restart.
            if (CurrentVersion < info.Version)
            {
                await ShowUpgradeNotificationDialogAsync(info);
            }
        }

        protected override void OnError(Exception e)
        {
            // TODO: Logging
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

    }
}
