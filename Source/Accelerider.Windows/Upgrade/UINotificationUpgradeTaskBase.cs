using System;
using System.Threading.Tasks;
using System.Windows;
using Accelerider.Windows.Infrastructure.Upgrade;
using Accelerider.Windows.ViewModels.Dialogs;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.Upgrade
{
    public abstract class UINotificationUpgradeTaskBase : UpgradeTaskBase
    {
        private bool _isUpgradeNotificationDialogOpened;

        protected UINotificationUpgradeTaskBase(string name, string installDirectory, string folderPrefix = null) : base(name, installDirectory, folderPrefix) { }

        protected async Task ShowUpgradeNotificationDialogAsync(UpgradeInfo info)
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
