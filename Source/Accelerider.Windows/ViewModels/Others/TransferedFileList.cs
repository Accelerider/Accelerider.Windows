using System.Collections.Generic;
using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.ViewModels.Others
{
    public class TransferedFileList : ObservableCollection<ITransferedFile>
    {
        public TransferedFileList(IEnumerable<ITransferedFile> files)
        {
            foreach (var file in files)
            {
                Add(file);
            }
        }

        protected override void InsertItem(int index, ITransferedFile item)
        {
            item.FileChekced += OnChecked;
            base.InsertItem(GetAppropriateIndex(item), item);
        }

        protected override void RemoveItem(int index)
        {
            Items[index].FileChekced -= OnChecked;
            base.RemoveItem(index);
        }

        private void OnChecked(object sender, FileCheckStatusEnum e)
        {
            var file = (ITransferedFile) sender;
            SetItem(IndexOf(file), file);
        }

        private int GetAppropriateIndex(ITransferedFile other)
        {
            int i = 0;
            for (; i < Items.Count; i++)
            {
                var item = Items[i];
                if (other.CompletedTime > item.CompletedTime) break;
            }
            return i;
        }
    }
}
