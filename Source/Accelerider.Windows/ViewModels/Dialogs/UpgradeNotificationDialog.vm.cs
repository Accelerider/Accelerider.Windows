using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Infrastructure.Upgrade;
using Unity;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class UpgradeNotificationDialogViewModel : ViewModelBase
    {

        private string _moduleIconUri;

        public string ModuleIconUri
        {
            get => _moduleIconUri;
            set => SetProperty(ref _moduleIconUri, value);
        }

        private string _moduleName;

        public string ModuleName
        {
            get => _moduleName;
            set => SetProperty(ref _moduleName, value);
        }

        private string _updateNotes;

        public string UpdateNotes
        {
            get => _updateNotes;
            set => SetProperty(ref _updateNotes, value);
        }

        public ICommand RestartCommand { get; }

        public UpgradeNotificationDialogViewModel(IUnityContainer container) : base(container)
        {
            ModuleIconUri = "pack://application:,,,/Accelerider.Windows.Assets;component/Images/logo-accelerider.png";

            RestartCommand = new RelayCommand(() => ProcessController.Restart());
        }

        public void Initialize(UpgradeInfo info)
        {
            ModuleName = $"{info.Name}-{info.Version.ToString(3)}";
            UpdateNotes = info.UpdateNotes;
        }
    }
}
