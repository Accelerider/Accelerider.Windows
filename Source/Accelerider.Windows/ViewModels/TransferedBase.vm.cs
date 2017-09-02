using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Infrastructure.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using Accelerider.Windows.Events;

namespace Accelerider.Windows.ViewModels
{
    public abstract class TransferedBaseViewModel<T> : ViewModelBase
        where T : TaskEndEvent, new()
    {
        private ObservableCollection<ITransferedFile> _transferedFiles;


        protected TransferedBaseViewModel(IUnityContainer container) : base(container)
        {
            TransferedFiles = new ObservableCollection<ITransferedFile>(GetTransferedFiles());
            EventAggregator.GetEvent<T>().Subscribe(token => Application.Current.Dispatcher.Invoke(() => OnGettingAToken(token)));
        }


        public ObservableCollection<ITransferedFile> TransferedFiles
        {
            get => _transferedFiles;
            set => SetProperty(ref _transferedFiles, value);
        }


        protected virtual void OnGettingAToken(ITransferTaskToken token)
        {
            TransferedFiles.Insert(0, token.GetTransferedFile());
        }


        protected abstract IReadOnlyCollection<ITransferedFile> GetTransferedFiles();
    }
}
