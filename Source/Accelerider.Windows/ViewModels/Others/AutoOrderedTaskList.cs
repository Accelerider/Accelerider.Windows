using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.ViewModels.Others
{
    public class AutoOrderedTaskList : ObservableCollection<TransferTaskViewModel>
    {
        public AutoOrderedTaskList(IEnumerable<TransferTaskViewModel> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        protected override void InsertItem(int index, TransferTaskViewModel item)
        {
            index = GetAppropriateIndex(item);
            if (index == -1) return;

            item.PropertyChanged += OnPropertyChanged;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base[index].PropertyChanged -= OnPropertyChanged;
            base.RemoveItem(index);
        }

        private readonly Dictionary<TransferTaskStatusEnum, int> _orderMapping = new Dictionary<TransferTaskStatusEnum, int>
        {
            { TransferTaskStatusEnum.Transfering, 0 },
            { TransferTaskStatusEnum.Waiting, 1 },
            { TransferTaskStatusEnum.Paused, 2 },
            { TransferTaskStatusEnum.Created, 3 },
        };

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(TransferTaskViewModel.TransferTaskStatus)) return;

            var item = (TransferTaskViewModel)sender;
            lock (item)
            {
                var appropriateIndex = GetAppropriateIndex(item);
                if (appropriateIndex == -1)
                {
                    Remove(item);
                    return;
                }

                var index = IndexOf(item);
                if (appropriateIndex != index)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        base.RemoveItem(index);
                        base.InsertItem(index < appropriateIndex ? appropriateIndex - 1 : appropriateIndex, item);
                    });
                }
            }
        }

        private int GetAppropriateIndex(TransferTaskViewModel other)
        {
            if (!_orderMapping.ContainsKey(other.TransferTaskStatus)) return -1;

            int i = 0;
            for (; i < Items.Count; i++)
            {
                var item = Items[i];
                if (_orderMapping[item.TransferTaskStatus] >= _orderMapping[other.TransferTaskStatus] &&
                    item.FileSummary.FilePath != other.FileSummary.FilePath)
                    break;
            }
            return i;
        }
    }
}
