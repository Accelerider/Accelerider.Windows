using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.TransportEngine;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Others
{
    public class TransferringTaskList : ObservableCollection<TransportingTaskItem>
    {
        public const string DownloadKey = "DownloadList";
        public const string UploadKey = "UploadList";
        private const int NotAvailable = -1;

        private readonly Dictionary<TransportStatus, int> _orderMapping = new Dictionary<TransportStatus, int>
        {
            { TransportStatus.Transporting, 0 },
            { TransportStatus.Ready, 1 },
            { TransportStatus.Faulted, 2 },
            { TransportStatus.Suspended, 3 },
        };

        private TransferredFileList _transferredFileList;

        public TransferringTaskList(IEnumerable<TransportingTaskItem> collection)
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

        protected override void InsertItem(int index, TransportingTaskItem item)
        {
            index = GetAppropriateIndex(item);
            if (index == NotAvailable)
            {
                AddToTransferredList(item);
                return;
            }

            item.Token.StatusChanged += OnTaskStatusChanged;
            Application.Current.Dispatcher.Invoke(() => base.InsertItem(index, item));
        }

        protected override void RemoveItem(int index)
        {
            base[index].Token.StatusChanged -= OnTaskStatusChanged;
            Application.Current.Dispatcher.Invoke(() => base.RemoveItem(index));
        }

        private void OnTaskStatusChanged(object sender, TransportStatus e)
        {
            var token = (ITaskReference)sender;
            var task = Items.First(item => EqualityComparer<ITaskReference>.Default.Equals(item.Token, token));
            lock (task)
            {
                var appropriateIndex = GetAppropriateIndex(task);
                if (appropriateIndex == NotAvailable)
                {
                    Remove(task);
                    AddToTransferredList(task);
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

        private void AddToTransferredList(TransportingTaskItem task)
        {
            if (task.TransferTaskStatus == TransportStatus.Completed)
            {
                Application.Current.Dispatcher.Invoke(() => TransferredFileList.Insert(0, task.Token.GetTransferredFile()));
            }
        }

        private int GetAppropriateIndex(TransportingTaskItem other)
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
