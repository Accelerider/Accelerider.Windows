using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Commands;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class SettingsPopupViewModel : ViewModelBase
    {
        private ICommand _openFolderDialogCommand;


        public SettingsPopupViewModel(IUnityContainer container) : base(container)
        {
            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogCommandExecute);
        }


        public ICommand OpenFolderDialogCommand
        {
            get => _openFolderDialogCommand;
            set => SetProperty(ref _openFolderDialogCommand, value);
        }


        private void OpenFolderDialogCommandExecute()
        {
            var dialog = new FolderBrowserDialog { Description = UiStrings.DownloadDialog_FolderBrowerDialogDescription };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //DownloadFolder = dialog.SelectedPath;
                //if (!DefaultFolders.Contains(DownloadFolder)) DefaultFolders.Insert(0, DownloadFolder);
            }
        }
    }
}
