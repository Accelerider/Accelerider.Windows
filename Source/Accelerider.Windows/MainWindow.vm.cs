using Accelerider.Windows.ViewModels;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.ViewModels.Others;

namespace Accelerider.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TransferingTaskList _downloadList;
        private TransferingTaskList _uploadList;


        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);

            ConfigureTransferList();
        }


        public TransferingTaskList DownloadList
        {
            get => _downloadList;
            set => SetProperty(ref _downloadList, value);
        }

        public TransferingTaskList UploadList
        {
            get => _uploadList;
            set => SetProperty(ref _uploadList, value);
        }


        public override void OnUnloaded(object view)
        {
            AcceleriderUser.OnExit();
        }


        private void ConfigureTransferList()
        {
            DownloadList = new TransferingTaskList(AcceleriderUser.GetDownloadingTasks().Select(task => new TransferingTaskViewModel(task)));
            UploadList = new TransferingTaskList(AcceleriderUser.GetUploadingTasks().Select(task => new TransferingTaskViewModel(task)));

            DownloadList.TransferedFileList = new TransferedFileList(AcceleriderUser.GetDownloadedFiles());
            UploadList.TransferedFileList = new TransferedFileList(AcceleriderUser.GetUploadedFiles());

            Container.RegisterInstance(TransferingTaskList.DownloadKey, DownloadList);
            Container.RegisterInstance(TransferingTaskList.UploadKey, UploadList);
        }
    }
}
