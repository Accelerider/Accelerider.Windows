using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Others
{
    public class TransferringTaskList : ObservableCollection<TransferringTaskViewModel>
    {
        public const string DownloadKey = "DownloadList";
        public const string UploadKey = "UploadList";
        private const int NotAvailable = -1;

        private readonly Dictionary<TransferTaskStatusEnum, int> _orderMapping = new Dictionary<TransferTaskStatusEnum, int>
        {
            { TransferTaskStatusEnum.Transferring, 0 },
            { TransferTaskStatusEnum.Waiting, 1 },
            { TransferTaskStatusEnum.Created, 1 },
            { TransferTaskStatusEnum.Faulted, 2 },
            { TransferTaskStatusEnum.Paused, 3 },
        };

        private TransferredFileList _transferredFileList;

        public TransferringTaskList(IEnumerable<TransferringTaskViewModel> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public TransferredFileList TransferredFileList
        {
            get => _transferredFileList;
            set
            {
                if (EqualityComparer<ObservableCollection<ITransferredFile>>.Default.Equals(_transferredFileList, value)) return;
                _transferredFileList = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(TransferredFileList)));
            }
        }

        protected override void InsertItem(int index, TransferringTaskViewModel item)
        {
            index = GetAppropriateIndex(item);
            if (index == NotAvailable)
            {
                AddToTransferredList(item);
                return;
            }

            item.Token.TransferTaskStatusChanged += OnTaskStatusChanged;
            Application.Current.Dispatcher.Invoke(() => base.InsertItem(index, item));
        }

        protected override void RemoveItem(int index)
        {
            base[index].Token.TransferTaskStatusChanged -= OnTaskStatusChanged;
            Application.Current.Dispatcher.Invoke(() => base.RemoveItem(index));
        }

        private void OnTaskStatusChanged(object sender, TransferTaskStatusChangedEventArgs e)
        {
            var token = (ITransferTaskToken)sender;
            var task = Items.First(item => EqualityComparer<ITransferTaskToken>.Default.Equals(item.Token, token));
            lock (task)
            {
                var appropriateIndex = GetAppropriateIndex(task);
                if (appropriateIndex == NotAvailable)
                {
                    Remove(task);
                    this.AddToTransferredList(task);
                    return;
                }

                var index = IndexOf(task);
                if (appropriateIndex != index)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        base.RemoveItem(index);
                        base.InsertItem(index < appropriateIndex ? --appropriateIndex : appropriateIndex, task);
                    });
                }
            }
        }

        private void AddToTransferredList(TransferringTaskViewModel task)
        {
            if (task.TransferTaskStatus == TransferTaskStatusEnum.Completed)
            {
                Application.Current.Dispatcher.Invoke(() => TransferredFileList.Insert(0, task.Token.GetTransferredFile()));
            }
        }

        private int GetAppropriateIndex(TransferringTaskViewModel other)
        {
            if (!_orderMapping.ContainsKey(other.TransferTaskStatus)) return NotAvailable;

            var i = 0;
            for (; i < Items.Count; i++)
            {
                var item = Items[i];
                var itemNext = i + 1 >= Items.Count ? item : Items[i + 1];
                if (_orderMapping[other.TransferTaskStatus] <= _orderMapping[item.TransferTaskStatus] &&
                    (item.FileSummary.FilePath != other.FileSummary.FilePath ||
                    _orderMapping[other.TransferTaskStatus] <= _orderMapping[itemNext.TransferTaskStatus]))
                    break;
            }

            return i;
        }
    }
}
