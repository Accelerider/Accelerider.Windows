using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Models;
using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public abstract class TransferredBaseViewModel : ViewModelBase, IViewLoadedAndUnloadedAware
    {
        protected Comparison<ILocalDiskFile> DefaultTransferredFileComparer { get; } =
            (x, y) => Comparer<DateTime>.Default.Compare(x.CompletedTime, y.CompletedTime);

        private ObservableSortedCollection<ILocalDiskFile> _transferredFiles;

        public ObservableSortedCollection<ILocalDiskFile> TransferredFiles
        {
            get => _transferredFiles;
            set => SetProperty(ref _transferredFiles, value);
        }

        protected TransferredBaseViewModel(IUnityContainer container) : base(container)
        {
        }

        public virtual void OnLoaded()
        {
        }

        public virtual void OnUnloaded()
        {
        }

        protected abstract ObservableSortedCollection<ILocalDiskFile> GetTransferredFiles();
    }
}
