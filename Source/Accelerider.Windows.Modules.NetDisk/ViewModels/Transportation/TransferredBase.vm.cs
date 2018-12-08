using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public abstract class TransferredBaseViewModel : ViewModelBase, IViewLoadedAndUnloadedAware
    {
        protected Comparison<ITransferredFile> DefaultTransferredFileComparer { get; } =
            (x, y) => Comparer<DateTime>.Default.Compare(x.CompletedTime, y.CompletedTime);

        private IEnumerable<ITransferredFile> _transferredFiles;


        protected TransferredBaseViewModel(IUnityContainer container) : base(container)
        {
        }

        public void OnLoaded()
        {
            TransferredFiles = GetTransferredFiles();
        }

        public void OnUnloaded()
        {
        }

        public IEnumerable<ITransferredFile> TransferredFiles
        {
            get => _transferredFiles;
            set => SetProperty(ref _transferredFiles, value);
        }

        protected abstract IEnumerable<ITransferredFile> GetTransferredFiles();
    }
}
