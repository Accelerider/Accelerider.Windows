using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.ViewModels;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Autofac;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public abstract class TransferredBaseViewModel : ViewModelBase, IAwareViewLoadedAndUnloaded
    {
        protected Comparison<ITransferredFile> DefaultTransferredFileComparer { get; } =
            (x, y) => Comparer<DateTime>.Default.Compare(x.CompletedTime, y.CompletedTime);

        private IEnumerable<ITransferredFile> _transferredFiles;


        protected TransferredBaseViewModel(IContainer container) : base(container)
        {
        }

        public void OnLoaded(object view)
        {
            TransferredFiles = GetTransferredFiles();
        }

        public void OnUnloaded(object view)
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
